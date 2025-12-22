using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FormBuilder.Services.Services
{
    public class FieldDataSourcesService : BaseService<FIELD_DATA_SOURCES, FieldDataSourceDto, CreateFieldDataSourceDto, UpdateFieldDataSourceDto>, IFieldDataSourcesService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IFieldDataSourcesRepository _fieldDataSourcesRepository;
        private readonly AkhmanageItContext _akhmanageItContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FieldDataSourcesService> _logger;

        public FieldDataSourcesService(
            IunitOfwork unitOfWork, 
            IFieldDataSourcesRepository fieldDataSourcesRepository, 
            AkhmanageItContext akhmanageItContext,
            IHttpClientFactory httpClientFactory,
            ILogger<FieldDataSourcesService> logger,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fieldDataSourcesRepository = fieldDataSourcesRepository ?? throw new ArgumentNullException(nameof(fieldDataSourcesRepository));
            _akhmanageItContext = akhmanageItContext ?? throw new ArgumentNullException(nameof(akhmanageItContext));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IBaseRepository<FIELD_DATA_SOURCES> Repository => _unitOfWork.FieldDataSourcesRepository;

        public async Task<ApiResponse> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var result = await base.GetByIdAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            var dataSources = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId);
            var dataSourceDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(dataSources);
            return new ApiResponse(200, "Field data sources retrieved successfully", dataSourceDtos);
        }

        public async Task<ApiResponse> GetActiveByFieldIdAsync(int fieldId)
        {
            var dataSources = await _fieldDataSourcesRepository.GetActiveByFieldIdAsync(fieldId);
            var dataSourceDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(dataSources);
            return new ApiResponse(200, "Active field data sources retrieved successfully", dataSourceDtos);
        }

        public async Task<ApiResponse> CreateAsync(CreateFieldDataSourceDto createDto)
        {
            // If ConfigurationJson is provided, use it; otherwise, build it from individual fields
            if (string.IsNullOrEmpty(createDto.ConfigurationJson))
            {
                createDto.ConfigurationJson = BuildConfigurationJson(createDto);
            }

            var result = await base.CreateAsync(createDto);
            return ConvertToApiResponse(result);
        }

        private string? BuildConfigurationJson(CreateFieldDataSourceDto dto)
        {
            try
            {
                if (dto.SourceType.Equals("LookupTable", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(dto.ApiUrl) && !string.IsNullOrEmpty(dto.ValuePath) && !string.IsNullOrEmpty(dto.TextPath))
                    {
                        var config = new
                        {
                            table = dto.ApiUrl,
                            valueColumn = dto.ValuePath,
                            textColumn = dto.TextPath
                        };
                        return System.Text.Json.JsonSerializer.Serialize(config);
                    }
                }
                else if (dto.SourceType.Equals("Api", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(dto.ApiUrl) && !string.IsNullOrEmpty(dto.ValuePath) && !string.IsNullOrEmpty(dto.TextPath))
                    {
                        var config = new Dictionary<string, object?>
                        {
                            ["url"] = dto.ApiUrl,
                            ["httpMethod"] = dto.HttpMethod ?? "GET",
                            ["valuePath"] = dto.ValuePath,
                            ["textPath"] = dto.TextPath
                        };
                        if (!string.IsNullOrEmpty(dto.RequestBodyJson))
                        {
                            config["requestBodyJson"] = dto.RequestBodyJson;
                        }
                        return System.Text.Json.JsonSerializer.Serialize(config);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to build ConfigurationJson from individual fields");
            }

            return null;
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFieldDataSourceDto dto)
        {
            // Validate if field exists
            var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(x => x.Id == dto.FieldId);
            if (!fieldExists)
                return ValidationResult.Failure("Invalid field ID");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> CreateBulkAsync(List<CreateFieldDataSourceDto> createDtos)
        {
            if (createDtos == null || !createDtos.Any())
                return new ApiResponse(400, "No field data sources provided");

            // Build ConfigurationJson for each DTO if not provided
            foreach (var dto in createDtos)
            {
                if (string.IsNullOrEmpty(dto.ConfigurationJson))
                {
                    dto.ConfigurationJson = BuildConfigurationJson(dto);
                }
            }

            // Validate all field IDs exist
            var fieldIds = createDtos.Select(d => d.FieldId).Distinct().ToList();
            foreach (var fieldId in fieldIds)
            {
                var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(f => f.Id == fieldId);
                if (!fieldExists)
                    return new ApiResponse(400, $"Invalid field ID: {fieldId}");
            }

            // Validate each DTO
            foreach (var dto in createDtos)
            {
                var validation = await ValidateCreateAsync(dto);
                if (!validation.IsValid)
                    return new ApiResponse(400, validation.ErrorMessage ?? "Validation failed");
            }

            var entities = _mapper.Map<List<FIELD_DATA_SOURCES>>(createDtos);
            foreach (var entity in entities)
            {
                entity.CreatedDate = entity.CreatedDate == default ? DateTime.UtcNow : entity.CreatedDate;
                entity.IsActive = true;
            }

            _unitOfWork.FieldDataSourcesRepository.AddRange(entities);
            await _unitOfWork.CompleteAsyn();

            var resultDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(entities);
            return new ApiResponse(200, "Field data sources created successfully", resultDtos);
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFieldDataSourceDto updateDto)
        {
            // If ConfigurationJson is provided, use it; otherwise, build it from individual fields
            if (string.IsNullOrEmpty(updateDto.ConfigurationJson))
            {
                // Convert UpdateFieldDataSourceDto to CreateFieldDataSourceDto for building JSON
                var createDto = new CreateFieldDataSourceDto
                {
                    SourceType = updateDto.SourceType,
                    ApiUrl = updateDto.ApiUrl,
                    HttpMethod = updateDto.HttpMethod,
                    RequestBodyJson = updateDto.RequestBodyJson,
                    ValuePath = updateDto.ValuePath,
                    TextPath = updateDto.TextPath
                };
                updateDto.ConfigurationJson = BuildConfigurationJson(createDto);
            }

            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var result = await base.SoftDeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByFieldIdAndTypeAsync(int fieldId, string sourceType)
        {
            var dataSource = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId, sourceType);
            if (dataSource == null)
                return new ApiResponse(404, "Field data source not found for the specified type");

            var dataSourceDto = _mapper.Map<FieldDataSourceDto>(dataSource);
            return new ApiResponse(200, "Field data source retrieved successfully", dataSourceDto);
        }

        public async Task<ApiResponse> GetDataSourcesCountAsync(int fieldId)
        {
            var count = await _fieldDataSourcesRepository.GetDataSourcesCountAsync(fieldId);
            return new ApiResponse(200, "Data sources count retrieved successfully", count);
        }

        // ================================
        // FIELD OPTIONS METHODS
        // ================================

        public async Task<ApiResponse> GetFieldOptionsAsync(int fieldId, Dictionary<string, object>? context = null, string? requestBodyJson = null)
        {
            try
            {
                // Get active data source for the field
                var dataSource = await _fieldDataSourcesRepository.GetActiveByFieldIdAsync(fieldId);
                var activeDataSource = dataSource.FirstOrDefault(ds => ds.IsActive);

                if (activeDataSource == null)
                {
                    // No data source configured, return static options from FIELD_OPTIONS
                    var staticOptions = await GetStaticOptionsAsync(fieldId);
                    return new ApiResponse(200, "Field options retrieved successfully", staticOptions);
                }

                // Load options based on source type
                List<FieldOptionResponseDto> options;
                switch (activeDataSource.SourceType.ToUpper())
                {
                    case "STATIC":
                        options = await GetStaticOptionsAsync(fieldId);
                        break;

                    case "LOOKUPTABLE":
                        // Check if options are cached in FIELD_OPTIONS, otherwise fetch and save
                        var lookupOptions = await GetLookupTableOptionsAsync(activeDataSource, context);
                        await SaveOptionsToDatabaseAsync(fieldId, lookupOptions, activeDataSource.Id);
                        options = lookupOptions;
                        break;

                    case "API":
                        // Check if options are cached in FIELD_OPTIONS, otherwise fetch and save
                        var apiOptions = await GetApiOptionsAsync(activeDataSource, requestBodyJson, context);
                        await SaveOptionsToDatabaseAsync(fieldId, apiOptions, activeDataSource.Id);
                        options = apiOptions;
                        break;

                    default:
                        return new ApiResponse(400, $"Unsupported source type: {activeDataSource.SourceType}");
                }

                return new ApiResponse(200, "Field options retrieved successfully", options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving field options for field {FieldId}", fieldId);
                return new ApiResponse(500, $"Error retrieving field options: {ex.Message}");
            }
        }

        public async Task<ApiResponse> PreviewDataSourceAsync(PreviewDataSourceRequestDto request)
        {
            try
            {
                List<FieldOptionResponseDto> options;
                switch (request.SourceType.ToUpper())
                {
                    case "STATIC":
                        if (request.FieldId.HasValue)
                        {
                            options = await GetStaticOptionsAsync(request.FieldId.Value);
                        }
                        else
                        {
                            return new ApiResponse(400, "FieldId is required for Static source type");
                        }
                        break;

                    case "LOOKUPTABLE":
                        if (string.IsNullOrEmpty(request.ApiUrl) || string.IsNullOrEmpty(request.ValuePath) || string.IsNullOrEmpty(request.TextPath))
                        {
                            return new ApiResponse(400, "Table name, ValuePath, and TextPath are required for LookupTable source type");
                        }
                        var lookupDataSource = new FIELD_DATA_SOURCES
                        {
                            ApiUrl = request.ApiUrl,
                            ValuePath = request.ValuePath,
                            TextPath = request.TextPath
                        };
                        options = await GetLookupTableOptionsAsync(lookupDataSource, null);
                        break;

                    case "API":
                        if (string.IsNullOrEmpty(request.ApiUrl) || string.IsNullOrEmpty(request.ValuePath) || string.IsNullOrEmpty(request.TextPath))
                        {
                            return new ApiResponse(400, "ApiUrl, ValuePath, and TextPath are required for API source type");
                        }
                        var apiDataSource = new FIELD_DATA_SOURCES
                        {
                            ApiUrl = request.ApiUrl,
                            HttpMethod = request.HttpMethod ?? "GET",
                            RequestBodyJson = request.RequestBodyJson,
                            ValuePath = request.ValuePath,
                            TextPath = request.TextPath
                        };
                        options = await GetApiOptionsAsync(apiDataSource, request.RequestBodyJson, null);
                        break;

                    default:
                        return new ApiResponse(400, $"Unsupported source type: {request.SourceType}");
                }

                return new ApiResponse(200, "Preview successful", options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing data source");
                return new ApiResponse(500, $"Error previewing data source: {ex.Message}");
            }
        }

        // ================================
        // PRIVATE HELPER METHODS
        // ================================

        private async Task<List<FieldOptionResponseDto>> GetStaticOptionsAsync(int fieldId)
        {
            var options = await _unitOfWork.FieldOptionsRepository.GetActiveByFieldIdAsync(fieldId);
            return options
                .OrderBy(o => o.OptionOrder)
                .Select(o => new FieldOptionResponseDto
                {
                    Value = o.OptionValue,
                    Text = o.OptionText
                })
                .ToList();
        }

        private async Task<List<FieldOptionResponseDto>> GetLookupTableOptionsAsync(FIELD_DATA_SOURCES dataSource, Dictionary<string, object>? context)
        {
            string tableName;
            string valueColumn;
            string textColumn;

            // Try to read from ConfigurationJson first
            if (!string.IsNullOrEmpty(dataSource.ConfigurationJson))
            {
                try
                {
                    var config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(dataSource.ConfigurationJson);
                    if (config != null)
                    {
                        tableName = config.ContainsKey("table") ? config["table"]?.ToString() ?? string.Empty : string.Empty;
                        valueColumn = config.ContainsKey("valueColumn") ? config["valueColumn"]?.ToString() ?? string.Empty : string.Empty;
                        textColumn = config.ContainsKey("textColumn") ? config["textColumn"]?.ToString() ?? string.Empty : string.Empty;
                    }
                    else
                    {
                        // Fallback to individual fields
                        tableName = dataSource.ApiUrl ?? string.Empty;
                        valueColumn = dataSource.ValuePath ?? string.Empty;
                        textColumn = dataSource.TextPath ?? string.Empty;
                    }
                }
                catch
                {
                    // Fallback to individual fields if JSON parsing fails
                    tableName = dataSource.ApiUrl ?? string.Empty;
                    valueColumn = dataSource.ValuePath ?? string.Empty;
                    textColumn = dataSource.TextPath ?? string.Empty;
                }
            }
            else
            {
                // Use individual fields
                tableName = dataSource.ApiUrl ?? string.Empty; // Table name is stored in ApiUrl
                valueColumn = dataSource.ValuePath ?? string.Empty; // Column name for value
                textColumn = dataSource.TextPath ?? string.Empty; // Column name for text
            }

            // Use reflection to dynamically access any DbSet in AkhmanageItContext
            var contextType = typeof(AkhmanageItContext);
            var dbSetProperty = contextType.GetProperty(tableName);

            if (dbSetProperty == null)
            {
                throw new ArgumentException($"Table '{tableName}' not found in AkhmanageItContext. Please check the table name.");
            }

            // Get the DbSet value from the context
            var dbSetValue = dbSetProperty.GetValue(_akhmanageItContext);
            if (dbSetValue == null)
            {
                throw new InvalidOperationException($"DbSet '{tableName}' is null in AkhmanageItContext");
            }

            // Get the generic type of the DbSet (e.g., TblCustomer)
            var dbSetType = dbSetProperty.PropertyType;
            if (!dbSetType.IsGenericType || dbSetType.GetGenericTypeDefinition() != typeof(DbSet<>))
            {
                throw new InvalidOperationException($"Property '{tableName}' is not a DbSet");
            }

            var entityType = dbSetType.GetGenericArguments()[0];

            // Use reflection to call QueryTableAsync with the correct type
            var method = typeof(FieldDataSourcesService)
                .GetMethod(nameof(QueryTableAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.MakeGenericMethod(entityType);

            if (method == null)
            {
                throw new InvalidOperationException("QueryTableAsync method not found");
            }

            var task = (Task<List<FieldOptionResponseDto>>)method.Invoke(this, new[] { dbSetValue, valueColumn, textColumn, context })!;
            return await task;
        }

        private async Task<List<FieldOptionResponseDto>> QueryTableAsync<T>(
            DbSet<T> dbSet, 
            string valueColumn, 
            string textColumn, 
            Dictionary<string, object>? context) where T : class
        {
            var query = dbSet.AsQueryable();
            var entityType = typeof(T);

            // Apply context filters if provided
            if (context != null)
            {
                // Filter by LegalEntityId if property exists
                if (context.ContainsKey("LegalEntityId"))
                {
                    var legalEntityId = Convert.ToInt32(context["LegalEntityId"]);
                    var property = entityType.GetProperty("IdLegalEntity") ?? 
                                  entityType.GetProperty("LegalEntityId");
                    if (property != null)
                    {
                        // Use expression tree for filtering
                        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "x");
                        var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
                        var constant = System.Linq.Expressions.Expression.Constant(legalEntityId);
                        var equals = System.Linq.Expressions.Expression.Equal(propertyAccess, constant);
                        var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equals, parameter);
                        query = query.Where(lambda);
                    }
                }
            }

            // Default: filter by IsActive if property exists
            var isActiveProperty = entityType.GetProperty("IsActive");
            if (isActiveProperty != null)
            {
                // Use compiled expression for IsActive filter
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "x");
                var property = System.Linq.Expressions.Expression.Property(parameter, isActiveProperty);
                var trueValue = System.Linq.Expressions.Expression.Constant(true);
                var equals = System.Linq.Expressions.Expression.Equal(property, trueValue);
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equals, parameter);
                query = query.Where(lambda);
            }

            var items = await query.ToListAsync();

            // Use reflection to get property values - try multiple possible column names (case-insensitive)
            var valueProperty = entityType.GetProperty(valueColumn, System.Reflection.BindingFlags.IgnoreCase | 
                                                                     System.Reflection.BindingFlags.Public | 
                                                                     System.Reflection.BindingFlags.Instance) ??
                               entityType.GetProperty("Id", System.Reflection.BindingFlags.IgnoreCase | 
                                                            System.Reflection.BindingFlags.Public | 
                                                            System.Reflection.BindingFlags.Instance) ??
                               entityType.GetProperty("ID", System.Reflection.BindingFlags.IgnoreCase | 
                                                            System.Reflection.BindingFlags.Public | 
                                                            System.Reflection.BindingFlags.Instance);

            var textProperty = entityType.GetProperty(textColumn, System.Reflection.BindingFlags.IgnoreCase | 
                                                                   System.Reflection.BindingFlags.Public | 
                                                                   System.Reflection.BindingFlags.Instance) ??
                               entityType.GetProperty("Name", System.Reflection.BindingFlags.IgnoreCase | 
                                                               System.Reflection.BindingFlags.Public | 
                                                               System.Reflection.BindingFlags.Instance) ??
                               entityType.GetProperty("Title", System.Reflection.BindingFlags.IgnoreCase | 
                                                                System.Reflection.BindingFlags.Public | 
                                                                System.Reflection.BindingFlags.Instance) ??
                               entityType.GetProperty("Code", System.Reflection.BindingFlags.IgnoreCase | 
                                                               System.Reflection.BindingFlags.Public | 
                                                               System.Reflection.BindingFlags.Instance);

            if (valueProperty == null)
            {
                var availableColumns = string.Join(", ", entityType.GetProperties().Select(p => p.Name));
                throw new ArgumentException($"Value column '{valueColumn}' not found in table. Available columns: {availableColumns}");
            }

            if (textProperty == null)
            {
                var availableColumns = string.Join(", ", entityType.GetProperties().Select(p => p.Name));
                throw new ArgumentException($"Text column '{textColumn}' not found in table. Available columns: {availableColumns}");
            }

            return items
                .Where(item => 
                {
                    var textValue = textProperty.GetValue(item)?.ToString();
                    return !string.IsNullOrWhiteSpace(textValue);
                })
                .Select(item => new FieldOptionResponseDto
                {
                    Value = valueProperty.GetValue(item)?.ToString() ?? "",
                    Text = textProperty.GetValue(item)?.ToString() ?? ""
                })
                .OrderBy(o => o.Text)
                .ToList();
        }

        private async Task<List<FieldOptionResponseDto>> GetApiOptionsAsync(
            FIELD_DATA_SOURCES dataSource, 
            string? requestBodyJson, 
            Dictionary<string, object>? context)
        {
            string apiUrl;
            string httpMethod;
            string valuePath;
            string textPath;
            string? requestBody;

            // Try to read from ConfigurationJson first
            if (!string.IsNullOrEmpty(dataSource.ConfigurationJson))
            {
                try
                {
                    var config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(dataSource.ConfigurationJson);
                    if (config != null)
                    {
                        apiUrl = config.ContainsKey("url") ? config["url"]?.ToString() ?? string.Empty : string.Empty;
                        httpMethod = config.ContainsKey("httpMethod") ? config["httpMethod"]?.ToString() ?? "GET" : "GET";
                        valuePath = config.ContainsKey("valuePath") ? config["valuePath"]?.ToString() ?? string.Empty : string.Empty;
                        textPath = config.ContainsKey("textPath") ? config["textPath"]?.ToString() ?? string.Empty : string.Empty;
                        requestBody = config.ContainsKey("requestBodyJson") ? config["requestBodyJson"]?.ToString() : null;
                    }
                    else
                    {
                        // Fallback to individual fields
                        apiUrl = dataSource.ApiUrl ?? string.Empty;
                        httpMethod = dataSource.HttpMethod ?? "GET";
                        valuePath = dataSource.ValuePath ?? string.Empty;
                        textPath = dataSource.TextPath ?? string.Empty;
                        requestBody = !string.IsNullOrEmpty(requestBodyJson) ? requestBodyJson : dataSource.RequestBodyJson;
                    }
                }
                catch
                {
                    // Fallback to individual fields if JSON parsing fails
                    apiUrl = dataSource.ApiUrl ?? string.Empty;
                    httpMethod = dataSource.HttpMethod ?? "GET";
                    valuePath = dataSource.ValuePath ?? string.Empty;
                    textPath = dataSource.TextPath ?? string.Empty;
                    requestBody = !string.IsNullOrEmpty(requestBodyJson) ? requestBodyJson : dataSource.RequestBodyJson;
                }
            }
            else
            {
                // Use individual fields
                apiUrl = dataSource.ApiUrl ?? string.Empty;
                httpMethod = dataSource.HttpMethod ?? "GET";
                valuePath = dataSource.ValuePath ?? string.Empty;
                textPath = dataSource.TextPath ?? string.Empty;
                requestBody = !string.IsNullOrEmpty(requestBodyJson) 
                    ? requestBodyJson 
                    : dataSource.RequestBodyJson;
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout

            HttpResponseMessage response;
            if (httpMethod.ToUpper() == "POST")
            {
                var body = !string.IsNullOrEmpty(requestBody) 
                    ? JsonSerializer.Deserialize<JsonElement>(requestBody) 
                    : (JsonElement?)null;

                response = await httpClient.PostAsJsonAsync(apiUrl, body);
            }
            else
            {
                response = await httpClient.GetAsync(apiUrl);
            }

            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            try
            {
                var jsonDoc = JsonDocument.Parse(jsonContent);
                var root = jsonDoc.RootElement;

                // Try to find array in response - support any API format
                JsonElement? arrayElement = null;

                // 1. Check if root is array directly
                if (root.ValueKind == JsonValueKind.Array)
                {
                    arrayElement = root;
                    _logger.LogInformation("Found array at root level");
                }
                // 2. Check if valuePath starts with array notation (e.g., "results[].id")
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    // Extract array path from valuePath (e.g., "results[].id" -> "results")
                    var arrayPath = ExtractArrayPath(valuePath, textPath);
                    
                    if (!string.IsNullOrEmpty(arrayPath))
                    {
                        _logger.LogInformation("Trying to navigate to array path: {ArrayPath}", arrayPath);
                        // Navigate to array using path
                        var element = NavigateToJsonElement(root, arrayPath);
                        if (element.HasValue && element.Value.ValueKind == JsonValueKind.Array)
                        {
                            arrayElement = element;
                            _logger.LogInformation("Found array at path: {ArrayPath}", arrayPath);
                        }
                    }
                    
                    // 3. If no array path found, try common property names
                    if (!arrayElement.HasValue)
                    {
                        var commonArrayNames = new[] { "data", "results", "items", "list", "records", "values", "content", "collection" };
                        foreach (var propName in commonArrayNames)
                        {
                            if (root.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.Array && prop.GetArrayLength() > 0)
                            {
                                arrayElement = prop;
                                _logger.LogInformation("Found array at property: {PropertyName}", propName);
                                break;
                            }
                        }
                    }
                    
                    // 4. Search recursively for first array found
                    if (!arrayElement.HasValue)
                    {
                        _logger.LogInformation("Searching recursively for array...");
                        arrayElement = FindFirstArray(root);
                        if (arrayElement.HasValue)
                        {
                            _logger.LogInformation("Found array recursively");
                        }
                    }
                }

                if (arrayElement.HasValue)
                {
                    // Clean paths from array notation before extracting values
                    var cleanValuePath = CleanPathFromArrayNotation(valuePath);
                    var cleanTextPath = CleanPathFromArrayNotation(textPath);
                    
                    _logger.LogInformation("Extracting options with valuePath: {ValuePath}, textPath: {TextPath}", cleanValuePath, cleanTextPath);
                    
                    return ExtractOptionsFromArray(arrayElement.Value, cleanValuePath, cleanTextPath);
                }

                // Log the actual response for debugging
                var structure = GetResponseStructure(root, 2);
                _logger.LogWarning("Could not find array in API response. Response structure: {Structure}", structure);
                throw new InvalidOperationException($"Invalid API response format. Could not find array in response. Response structure: {structure}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON response from API: {ApiUrl}", apiUrl);
                throw new InvalidOperationException($"Invalid JSON response from API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing API response from: {ApiUrl}", apiUrl);
                throw;
            }
        }

        private List<FieldOptionResponseDto> ExtractOptionsFromArray(JsonElement arrayElement, string valuePath, string textPath)
        {
            var options = new List<FieldOptionResponseDto>();
            var itemCount = 0;
            var skippedCount = 0;

            // Note: paths are already cleaned before calling this method
            _logger.LogInformation("Extracting options from array - valuePath: '{ValuePath}', textPath: '{TextPath}'", 
                valuePath, textPath);

            foreach (var item in arrayElement.EnumerateArray())
            {
                itemCount++;
                var value = GetJsonValue(item, valuePath);
                var text = GetJsonValue(item, textPath);

                // Log first 3 items for debugging
                if (itemCount <= 3)
                {
                    _logger.LogInformation("Item {ItemCount} - valuePath: '{ValuePath}', extracted value: '{Value}', textPath: '{TextPath}', extracted text: '{Text}'", 
                        itemCount, valuePath, value ?? "null", textPath, text ?? "null");
                    
                    // Log item structure for first item
                    if (itemCount == 1)
                    {
                        var itemStructure = GetResponseStructure(item, 2);
                        _logger.LogInformation("First item structure: {ItemStructure}", itemStructure);
                    }
                }

                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text))
                {
                    options.Add(new FieldOptionResponseDto
                    {
                        Value = value.Trim(),
                        Text = text.Trim()
                    });
                }
                else
                {
                    skippedCount++;
                    if (skippedCount <= 5) // Log first 5 skipped items
                    {
                        var itemStructure = GetResponseStructure(item, 1);
                        _logger.LogWarning("Skipped item {ItemCount} - value: '{Value}', text: '{Text}'. Item structure: {ItemStructure}", 
                            itemCount, value ?? "null", text ?? "null", itemStructure);
                    }
                }
            }

            _logger.LogInformation("Extracted {Count} options from {Total} items (skipped {Skipped})", 
                options.Count, itemCount, skippedCount);

            if (options.Count == 0 && itemCount > 0)
            {
                _logger.LogError("No options extracted! Check valuePath and textPath. First item structure: {ItemStructure}", 
                    GetResponseStructure(arrayElement.EnumerateArray().First(), 2));
            }

            return options;
        }

        private string CleanPathFromArrayNotation(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // Remove array notation if present (e.g., "results[].id" -> "id" or "results.id")
            // Handle patterns like "results[].id", "data[].name", etc.
            var pattern = @"^[^\[\]]+\[\]\.";
            path = Regex.Replace(path, pattern, "");
            
            // Also handle if path starts with array notation
            if (path.StartsWith("[]."))
            {
                path = path.Substring(3);
            }
            else if (path.Contains("[]"))
            {
                path = path.Replace("[]", "");
            }

            return path;
        }

        private string GetJsonValue(JsonElement element, string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            // Note: path should already be cleaned from array notation before calling this method
            var originalPath = path;

            // Simple path navigation (e.g., "id" or "name.first" or "login.uuid")
            var parts = path.Split('.');
            var current = element;

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                // Try exact match first (case-sensitive)
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var property))
                {
                    current = property;
                    continue;
                }

                // Try case-insensitive match
                if (current.ValueKind == JsonValueKind.Object)
                {
                    var found = false;
                    foreach (var prop in current.EnumerateObject())
                    {
                        if (string.Equals(prop.Name, part, StringComparison.OrdinalIgnoreCase))
                        {
                            current = prop.Value;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // Log available properties for debugging
                        var availableProps = string.Join(", ", current.EnumerateObject().Select(p => p.Name));
                        _logger.LogWarning("Property '{Part}' not found in path '{Path}'. Available properties: {Properties}", 
                            part, originalPath, availableProps);
                        return "";
                    }
                }
                else
                {
                    _logger.LogWarning("Cannot navigate to '{Part}' in path '{Path}'. Current element is not an object (ValueKind: {ValueKind})", 
                        part, originalPath, current.ValueKind);
                    return "";
                }
            }

            // Extract value based on type
            var result = current.ValueKind switch
            {
                JsonValueKind.String => current.GetString() ?? "",
                JsonValueKind.Number => current.GetRawText().Trim(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "",
                JsonValueKind.Undefined => "",
                _ => current.GetRawText().Trim()
            };

            return result;
        }

        private string? ExtractArrayPath(string valuePath, string textPath)
        {
            // Extract array path from paths like "results[].id" or "data[].name"
            var paths = new[] { valuePath, textPath };
            
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;

                // Check for array notation
                var arrayMatch = Regex.Match(path, @"^([^\[\]]+)\[\]");
                if (arrayMatch.Success)
                {
                    return arrayMatch.Groups[1].Value;
                }
            }

            return null;
        }

        private JsonElement? NavigateToJsonElement(JsonElement root, string path)
        {
            if (string.IsNullOrEmpty(path))
                return root;

            var parts = path.Split('.');
            var current = root;

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var property))
                {
                    current = property;
                }
                else
                {
                    return null;
                }
            }

            return current;
        }

        private JsonElement? FindFirstArray(JsonElement element, int maxDepth = 5, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth)
                return null;

            // Recursively search for first array
            if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > 0)
            {
                return element;
            }

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    // Skip metadata properties
                    if (prop.Name.Equals("info", StringComparison.OrdinalIgnoreCase) ||
                        prop.Name.Equals("meta", StringComparison.OrdinalIgnoreCase) ||
                        prop.Name.Equals("pagination", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var result = FindFirstArray(prop.Value, maxDepth, currentDepth + 1);
                    if (result.HasValue)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private string GetResponseStructure(JsonElement element, int maxDepth = 3, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth)
                return "...";

            return element.ValueKind switch
            {
                JsonValueKind.Array => $"[Array with {element.GetArrayLength()} items]",
                JsonValueKind.Object => $"{{ {string.Join(", ", element.EnumerateObject().Take(5).Select(p => $"\"{p.Name}\": {GetResponseStructure(p.Value, maxDepth, currentDepth + 1)}"))} }}",
                JsonValueKind.String => $"\"{element.GetString()?.Substring(0, Math.Min(50, element.GetString()?.Length ?? 0))}...\"",
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "null",
                _ => element.GetRawText()
            };
        }

        // ================================
        // GET AVAILABLE LOOKUP TABLES
        // ================================
        public async Task<ApiResponse> GetAvailableLookupTablesAsync()
        {
            try
            {
                var contextType = typeof(AkhmanageItContext);
                var suitableTables = new List<object>();

                var dbSetProperties = contextType.GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && 
                               p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

                foreach (var prop in dbSetProperties)
                {
                    var entityType = prop.PropertyType.GetGenericArguments()[0];
                    var properties = entityType.GetProperties();

                    // Check if entity has Id property (or ID, Id, etc.)
                    var hasIdProperty = properties.Any(p => 
                        p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase));

                    // Check if entity has Name property (or Name, Title, Description, etc.)
                    var hasNameProperty = properties.Any(p => 
                        p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("Title", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("Description", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("Code", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("Text", StringComparison.OrdinalIgnoreCase));

                    // Only include tables that have both Id and Name-like properties
                    if (hasIdProperty && hasNameProperty)
                    {
                        // Get the actual property names
                        var idProperty = properties.FirstOrDefault(p => 
                            p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase));
                        
                        var nameProperty = properties.FirstOrDefault(p => 
                            p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase)) ??
                            properties.FirstOrDefault(p => 
                                p.Name.Equals("Title", StringComparison.OrdinalIgnoreCase)) ??
                            properties.FirstOrDefault(p => 
                                p.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)) ??
                            properties.FirstOrDefault(p => 
                                p.Name.Equals("Code", StringComparison.OrdinalIgnoreCase)) ??
                            properties.FirstOrDefault(p => 
                                p.Name.Equals("Text", StringComparison.OrdinalIgnoreCase));

                        suitableTables.Add(new
                        {
                            Name = prop.Name,
                            EntityType = entityType.Name,
                            IdColumn = idProperty?.Name ?? "Id",
                            NameColumn = nameProperty?.Name ?? "Name",
                            DisplayName = $"{prop.Name} ({entityType.Name})"
                        });
                    }
                }

                var result = suitableTables
                    .OrderBy(t => ((dynamic)t).Name)
                    .ToList();

                return new ApiResponse(200, "Available lookup tables retrieved successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available lookup tables");
                return new ApiResponse(500, $"Error retrieving available lookup tables: {ex.Message}");
            }
        }

        // ================================
        // HELPER METHODS
        // ================================
        private ApiResponse ConvertToApiResponse<T>(ServiceResult<T> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }

        private ApiResponse ConvertToApiResponse(ServiceResult<bool> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }

        // ================================
        // SAVE OPTIONS TO DATABASE
        // ================================
        private async Task SaveOptionsToDatabaseAsync(int fieldId, List<FieldOptionResponseDto> options, int dataSourceId)
        {
            try
            {
                // Get existing options for this field
                var existingOptions = await _unitOfWork.FieldOptionsRepository.GetByFieldIdAsync(fieldId);
                var dataSource = await _fieldDataSourcesRepository.GetByIdAsync(dataSourceId);
                
                if (dataSource != null)
                {
                    // Delete options that were created after or at the same time as the data source
                    // This helps identify options that came from data sources
                    var optionsToDelete = existingOptions
                        .Where(opt => opt.CreatedDate >= dataSource.CreatedDate)
                        .ToList();

                    foreach (var opt in optionsToDelete)
                    {
                        _unitOfWork.FieldOptionsRepository.Delete(opt);
                    }
                }

                // Add new options
                int order = 1;
                foreach (var option in options)
                {
                    var fieldOption = new FormBuilder.Domian.Entitys.froms.FIELD_OPTIONS
                    {
                        FieldId = fieldId,
                        OptionValue = option.Value?.ToString() ?? "",
                        OptionText = option.Text ?? "",
                        OptionOrder = order++,
                        IsActive = true,
                        IsDefault = false,
                        CreatedDate = DateTime.UtcNow
                    };

                    _unitOfWork.FieldOptionsRepository.Add(fieldOption);
                }

                await _unitOfWork.CompleteAsyn();
                _logger.LogInformation("Saved {Count} options to database for field {FieldId} from data source {DataSourceId}", 
                    options.Count, fieldId, dataSourceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving options to database for field {FieldId}", fieldId);
                // Don't throw - we still want to return the options even if saving fails
            }
        }
    }
}

