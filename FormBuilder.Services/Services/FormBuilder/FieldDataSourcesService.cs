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
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

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
            
            // If DataSource is Api or LookupTable, delete all existing options for this field
            if (result.Success && (string.Equals(createDto.SourceType, "Api", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(createDto.SourceType, "LookupTable", StringComparison.OrdinalIgnoreCase)))
            {
                await DeleteAllOptionsForFieldAsync(createDto.FieldId);
            }
            
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
                        if (!string.IsNullOrEmpty(dto.ApiPath))
                        {
                            config["apiPath"] = dto.ApiPath;
                        }
                        if (!string.IsNullOrEmpty(dto.RequestBodyJson))
                        {
                            config["requestBodyJson"] = dto.RequestBodyJson;
                        }
                        if (dto.ArrayPropertyNames != null && dto.ArrayPropertyNames.Any())
                        {
                            config["arrayPropertyNames"] = dto.ArrayPropertyNames;
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
            // Get existing DataSource to check if sourceType changed
            var existingDataSource = await Repository.SingleOrDefaultAsync(e => e.Id == id, asNoTracking: true);
            string? oldSourceType = existingDataSource?.SourceType;
            
            // If ConfigurationJson is provided, use it; otherwise, build it from individual fields
            if (string.IsNullOrEmpty(updateDto.ConfigurationJson))
            {
                // Convert UpdateFieldDataSourceDto to CreateFieldDataSourceDto for building JSON
                var createDto = new CreateFieldDataSourceDto
                {
                    SourceType = updateDto.SourceType,
                    ApiUrl = updateDto.ApiUrl,
                    ApiPath = updateDto.ApiPath,
                    HttpMethod = updateDto.HttpMethod,
                    RequestBodyJson = updateDto.RequestBodyJson,
                    ValuePath = updateDto.ValuePath,
                    TextPath = updateDto.TextPath
                };
                updateDto.ConfigurationJson = BuildConfigurationJson(createDto);
            }

            var result = await base.UpdateAsync(id, updateDto);
            
            // If sourceType changed from Static to Api/LookupTable, delete all options
            if (result.Success && existingDataSource != null)
            {
                bool wasStatic = string.Equals(oldSourceType, "Static", StringComparison.OrdinalIgnoreCase);
                bool isNowApiOrLookup = string.Equals(updateDto.SourceType, "Api", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(updateDto.SourceType, "LookupTable", StringComparison.OrdinalIgnoreCase);
                
                if (wasStatic && isNowApiOrLookup)
                {
                    await DeleteAllOptionsForFieldAsync(existingDataSource.FieldId);
                }
                // Also delete if directly setting to Api/LookupTable (even if old was null)
                else if (isNowApiOrLookup && (oldSourceType == null || !string.Equals(oldSourceType, "Api", StringComparison.OrdinalIgnoreCase) && 
                                               !string.Equals(oldSourceType, "LookupTable", StringComparison.OrdinalIgnoreCase)))
                {
                    await DeleteAllOptionsForFieldAsync(existingDataSource.FieldId);
                }
            }
            
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
                List<FieldOptionResponseDto> responseOptions;
                switch (activeDataSource.SourceType.ToUpper())
                {
                    case "STATIC":
                        responseOptions = await GetStaticOptionsAsync(fieldId);
                        break;

                    case "LOOKUPTABLE":
                        // For LookupTable, fetch options from database table (do NOT save to FIELD_OPTIONS)
                        responseOptions = await GetLookupTableOptionsAsync(activeDataSource, context);
                        break;

                    case "API":
                        // For Api, fetch options from external API (do NOT save to FIELD_OPTIONS)
                        // Get custom array property names from ConfigurationJson or DTO
                        List<string>? customArrayNames = null;
                        if (!string.IsNullOrEmpty(activeDataSource.ConfigurationJson))
                        {
                            try
                            {
                                var config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(activeDataSource.ConfigurationJson);
                                if (config != null && config.ContainsKey("arrayPropertyNames"))
                                {
                                    var arrayNamesJson = config["arrayPropertyNames"]?.ToString();
                                    if (!string.IsNullOrEmpty(arrayNamesJson))
                                    {
                                        customArrayNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(arrayNamesJson);
                                    }
                                }
                            }
                            catch { }
                        }
                        responseOptions = await GetApiOptionsAsync(activeDataSource, requestBodyJson, context, customArrayNames);
                        break;

                    default:
                        return new ApiResponse(400, $"Unsupported source type: {activeDataSource.SourceType}");
                }

                // Return FieldOptionResponseDto (with 'text' and 'value' properties) for frontend compatibility
                return new ApiResponse(200, "Field options retrieved successfully", responseOptions);
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
                        if (string.IsNullOrEmpty(request.ApiUrl))
                        {
                            return new ApiResponse(400, "ApiUrl is required for API source type");
                        }
                        
                        // If valuePath or textPath are not provided, auto-detect them using InspectApi
                        string valuePath = request.ValuePath ?? string.Empty;
                        string textPath = request.TextPath ?? string.Empty;
                        
                        if (string.IsNullOrEmpty(valuePath) || string.IsNullOrEmpty(textPath))
                        {
                            _logger.LogInformation("valuePath or textPath not provided, auto-detecting from API structure...");
                            
                            // Auto-inspect API to get suggested paths
                            var inspectRequest = new InspectApiRequestDto
                            {
                                ApiUrl = request.ApiUrl,
                                ApiPath = request.ApiPath,
                                HttpMethod = request.HttpMethod ?? "GET",
                                RequestBodyJson = request.RequestBodyJson,
                                ArrayPropertyNames = request.ArrayPropertyNames
                            };
                            
                            var inspectResult = await InspectApiAsync(inspectRequest);
                            
                            if (inspectResult.StatusCode == 200 && inspectResult.Data != null)
                            {
                                var inspectionData = System.Text.Json.JsonSerializer.Deserialize<ApiInspectionResponseDto>(
                                    System.Text.Json.JsonSerializer.Serialize(inspectResult.Data));
                                
                                if (inspectionData != null && inspectionData.Success)
                                {
                                    // Use suggested paths if available
                                    if (string.IsNullOrEmpty(valuePath) && inspectionData.SuggestedValuePaths.Any())
                                    {
                                        valuePath = inspectionData.SuggestedValuePaths.First();
                                        _logger.LogInformation("Auto-detected valuePath: {ValuePath}", valuePath);
                                    }
                                    
                                    if (string.IsNullOrEmpty(textPath) && inspectionData.SuggestedTextPaths.Any())
                                    {
                                        textPath = inspectionData.SuggestedTextPaths.First();
                                        _logger.LogInformation("Auto-detected textPath: {TextPath}", textPath);
                                    }
                                    
                                    // If still empty, try availableFields
                                    if (string.IsNullOrEmpty(valuePath) && inspectionData.AvailableFields.Any())
                                    {
                                        valuePath = inspectionData.AvailableFields.First();
                                        _logger.LogInformation("Using first available field as valuePath: {ValuePath}", valuePath);
                                    }
                                    
                                    if (string.IsNullOrEmpty(textPath) && inspectionData.AvailableFields.Count > 1)
                                    {
                                        textPath = inspectionData.AvailableFields.Skip(1).First();
                                        _logger.LogInformation("Using second available field as textPath: {TextPath}", textPath);
                                    }
                                    
                                    // If still empty, return helpful error with available fields
                                    if (string.IsNullOrEmpty(valuePath) || string.IsNullOrEmpty(textPath))
                                    {
                                        var availableFieldsMsg = inspectionData.AvailableFields.Any() 
                                            ? $"Available fields: {string.Join(", ", inspectionData.AvailableFields)}" 
                                            : "No fields found in API response";
                                        var nestedFieldsMsg = inspectionData.NestedFields.Any() 
                                            ? $"Nested fields: {string.Join(", ", inspectionData.NestedFields)}" 
                                            : "";
                                        
                                        return new ApiResponse(400, 
                                            $"Could not auto-detect valuePath or textPath. {availableFieldsMsg}. {nestedFieldsMsg}. " +
                                            $"Please provide valuePath and textPath manually, or use /inspect-api endpoint first to see available fields.");
                                    }
                                }
                                else
                                {
                                    return new ApiResponse(400, 
                                        $"Failed to inspect API structure: {inspectionData?.ErrorMessage ?? "Unknown error"}. " +
                                        $"Please provide valuePath and textPath manually.");
                                }
                            }
                            else
                            {
                                return new ApiResponse(400, 
                                    "Could not auto-detect valuePath and textPath. Please provide them manually or use /inspect-api endpoint first.");
                            }
                        }
                        
                        var apiDataSource = new FIELD_DATA_SOURCES
                        {
                            ApiUrl = request.ApiUrl,
                            ApiPath = request.ApiPath,
                            HttpMethod = request.HttpMethod ?? "GET",
                            RequestBodyJson = request.RequestBodyJson,
                            ValuePath = valuePath,
                            TextPath = textPath
                        };
                        options = await GetApiOptionsAsync(apiDataSource, request.RequestBodyJson, null, request.ArrayPropertyNames);
                        break;

                    default:
                        return new ApiResponse(400, $"Unsupported source type: {request.SourceType}");
                }

                return new ApiResponse(200, "Preview successful", options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing data source");
                // Ensure error message is safe for JSON serialization
                var errorMessage = SanitizeForJson(ex.Message ?? "An unknown error occurred");
                return new ApiResponse(500, $"Error previewing data source: {errorMessage}");
            }
        }

        // ================================
        // PRIVATE HELPER METHODS
        // ================================

        /// <summary>
        /// Sanitizes a string to be safe for JSON serialization by removing problematic characters
        /// </summary>
        private string SanitizeForJson(string? input, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Limit length to prevent huge error messages
            var sanitized = input.Length > maxLength 
                ? input.Substring(0, maxLength) + "..." 
                : input;

            // Remove null bytes and other control characters that can corrupt JSON
            sanitized = sanitized.Replace("\0", ""); // Remove null bytes
            
            // Remove other problematic control characters but keep newlines and tabs for readability
            var sb = new StringBuilder();
            foreach (char c in sanitized)
            {
                // Keep printable characters, newlines, tabs, and common punctuation
                if (char.IsLetterOrDigit(c) || 
                    char.IsPunctuation(c) || 
                    char.IsWhiteSpace(c) || 
                    c == '\n' || c == '\r' || c == '\t' ||
                    (c >= 32 && c <= 126)) // ASCII printable range
                {
                    sb.Append(c);
                }
                else
                {
                    // Replace problematic characters with space
                    sb.Append(' ');
                }
            }

            return sb.ToString().Trim();
        }

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

            // 1. Parse configuration from ConfigurationJson or individual fields
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
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Invalid JSON in ConfigurationJson for field data source {DataSourceId}. Error: {ErrorMessage}", 
                        dataSource.Id, ex.Message);
                    throw new ArgumentException($"Invalid JSON in ConfigurationJson: {ex.Message}. Please check the JSON format.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing ConfigurationJson for field data source {DataSourceId}", dataSource.Id);
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

            // 2. Validate required fields
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name is required for LookupTable source type. Please provide it in ApiUrl or ConfigurationJson.");
            }

            if (string.IsNullOrWhiteSpace(valueColumn))
            {
                throw new ArgumentException("Value column is required for LookupTable source type. Please provide it in ValuePath or ConfigurationJson.");
            }

            if (string.IsNullOrWhiteSpace(textColumn))
            {
                throw new ArgumentException("Text column is required for LookupTable source type. Please provide it in TextPath or ConfigurationJson.");
            }

            // 3. Check database connection
            try
            {
                if (!await _akhmanageItContext.Database.CanConnectAsync())
                {
                    _logger.LogError("Database connection failed for LookupTable query");
                    throw new InvalidOperationException("Database connection failed. Please check your database connection.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database connection");
                throw new InvalidOperationException($"Database connection error: {ex.Message}");
            }

            // 4. Try to use reflection to access DbSet (preferred method)
            try
            {
                var contextType = typeof(AkhmanageItContext);
                var dbSetProperty = contextType.GetProperty(tableName, System.Reflection.BindingFlags.IgnoreCase | 
                                                                        System.Reflection.BindingFlags.Public | 
                                                                        System.Reflection.BindingFlags.Instance);

                if (dbSetProperty == null)
                {
                    // Try to find table with different case
                    var allProperties = contextType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    var matchingProperty = allProperties.FirstOrDefault(p => 
                        string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase));
                    
                    if (matchingProperty == null)
                    {
                        var availableTables = string.Join(", ", allProperties
                            .Where(p => p.PropertyType.IsGenericType && 
                                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                            .Select(p => p.Name));
                        
                        _logger.LogError("Table '{TableName}' not found in AkhmanageItContext. Available tables: {AvailableTables}", 
                            tableName, availableTables);
                        throw new ArgumentException($"Table '{tableName}' not found in AkhmanageItContext. Available tables: {availableTables}");
                    }
                    
                    dbSetProperty = matchingProperty;
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
            catch (ArgumentException)
            {
                // Re-throw validation errors
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-throw operation errors
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing table '{TableName}' using reflection. Attempting SQL fallback.", tableName);
                
                // 5. Fallback to SQL Raw Query if reflection fails
                try
                {
                    return await QueryTableUsingSqlAsync(tableName, valueColumn, textColumn, context);
                }
                catch (Exception sqlEx)
                {
                    _logger.LogError(sqlEx, "SQL fallback also failed for table '{TableName}'", tableName);
                    throw new InvalidOperationException($"Failed to query table '{tableName}'. Reflection error: {ex.Message}. SQL error: {sqlEx.Message}");
                }
            }
        }

        /// <summary>
        /// Fallback method to query table using raw SQL if reflection fails
        /// </summary>
        private async Task<List<FieldOptionResponseDto>> QueryTableUsingSqlAsync(
            string tableName, 
            string valueColumn, 
            string textColumn, 
            Dictionary<string, object>? context)
        {
            // Validate table and column names to prevent SQL injection
            if (!IsValidIdentifier(tableName) || !IsValidIdentifier(valueColumn) || !IsValidIdentifier(textColumn))
            {
                throw new ArgumentException("Invalid table or column name. Only alphanumeric characters and underscores are allowed.");
            }

            // Build SQL query with parameterized values to prevent SQL injection
            var sql = $@"
                SELECT 
                    [{valueColumn}] AS Value, 
                    [{textColumn}] AS Text 
                FROM [{tableName}] 
                WHERE 1=1";

            // Add IsActive filter if column exists (check first using connection)
            var connection = _akhmanageItContext.Database.GetDbConnection();
            var hasIsActiveColumn = false;
            var hasLegalEntityColumn = false;

            try
            {
                await connection.OpenAsync();
                
                // Check if IsActive column exists
                using (var checkCommand = connection.CreateCommand())
                {
                    checkCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = 'IsActive'";
                    var tableParam = checkCommand.CreateParameter();
                    tableParam.ParameterName = "@tableName";
                    tableParam.Value = tableName;
                    checkCommand.Parameters.Add(tableParam);
                    
                    var result = await checkCommand.ExecuteScalarAsync();
                    hasIsActiveColumn = Convert.ToInt32(result) > 0;
                }

                // Check if LegalEntityId column exists
                if (context != null && context.ContainsKey("LegalEntityId"))
                {
                    using (var checkCommand = connection.CreateCommand())
                    {
                        checkCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND (COLUMN_NAME = 'IdLegalEntity' OR COLUMN_NAME = 'LegalEntityId')";
                        var tableParam = checkCommand.CreateParameter();
                        tableParam.ParameterName = "@tableName";
                        tableParam.Value = tableName;
                        checkCommand.Parameters.Add(tableParam);
                        
                        var result = await checkCommand.ExecuteScalarAsync();
                        hasLegalEntityColumn = Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not verify column existence for table '{TableName}'", tableName);
            }

            // Add IsActive filter if column exists
            if (hasIsActiveColumn)
            {
                sql += " AND [IsActive] = 1";
            }

            // Add context filters
            if (hasLegalEntityColumn && context != null && context.ContainsKey("LegalEntityId"))
            {
                try
                {
                    var legalEntityId = Convert.ToInt32(context["LegalEntityId"]);
                    sql += $" AND ([IdLegalEntity] = {legalEntityId} OR [LegalEntityId] = {legalEntityId})";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not apply LegalEntityId filter for table '{TableName}'", tableName);
                }
            }

            sql += $" ORDER BY [{textColumn}]";

            // Execute query using ADO.NET directly since EF Core doesn't have SqlQueryRaw for arbitrary types
            var options = new List<FieldOptionResponseDto>();

            try
            {
                // Connection is already open from column check above
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = sql;

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    options.Add(new FieldOptionResponseDto
                    {
                        Value = reader["Value"]?.ToString() ?? "",
                        Text = reader["Text"]?.ToString() ?? ""
                    });
                }
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return options;
        }

        /// <summary>
        /// Validates that a string is a valid SQL identifier (prevents SQL injection)
        /// </summary>
        private bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return false;

            // Only allow alphanumeric characters, underscores, and brackets
            return System.Text.RegularExpressions.Regex.IsMatch(identifier, @"^[a-zA-Z0-9_\[\]]+$");
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
            Dictionary<string, object>? context,
            List<string>? customArrayPropertyNames = null)
        {
            string baseUrl;
            string? apiPath;
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
                        baseUrl = config.ContainsKey("url") ? config["url"]?.ToString() ?? string.Empty : string.Empty;
                        apiPath = config.ContainsKey("apiPath") ? config["apiPath"]?.ToString() : null;
                        httpMethod = config.ContainsKey("httpMethod") ? config["httpMethod"]?.ToString() ?? "GET" : "GET";
                        valuePath = config.ContainsKey("valuePath") ? config["valuePath"]?.ToString() ?? string.Empty : string.Empty;
                        textPath = config.ContainsKey("textPath") ? config["textPath"]?.ToString() ?? string.Empty : string.Empty;
                        requestBody = config.ContainsKey("requestBodyJson") ? config["requestBodyJson"]?.ToString() : null;
                        
                        // Get custom array property names from config if not provided as parameter
                        if (customArrayPropertyNames == null && config.ContainsKey("arrayPropertyNames"))
                        {
                            try
                            {
                                var arrayNamesJson = config["arrayPropertyNames"];
                                if (arrayNamesJson != null)
                                {
                                    customArrayPropertyNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(arrayNamesJson.ToString() ?? "[]");
                                }
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        // Fallback to individual fields
                        baseUrl = dataSource.ApiUrl ?? string.Empty;
                        apiPath = dataSource.ApiPath;
                        httpMethod = dataSource.HttpMethod ?? "GET";
                        valuePath = dataSource.ValuePath ?? string.Empty;
                        textPath = dataSource.TextPath ?? string.Empty;
                        requestBody = !string.IsNullOrEmpty(requestBodyJson) ? requestBodyJson : dataSource.RequestBodyJson;
                    }
                }
                catch
                {
                    // Fallback to individual fields if JSON parsing fails
                    baseUrl = dataSource.ApiUrl ?? string.Empty;
                    apiPath = dataSource.ApiPath;
                    httpMethod = dataSource.HttpMethod ?? "GET";
                    valuePath = dataSource.ValuePath ?? string.Empty;
                    textPath = dataSource.TextPath ?? string.Empty;
                    requestBody = !string.IsNullOrEmpty(requestBodyJson) ? requestBodyJson : dataSource.RequestBodyJson;
                }
            }
            else
            {
                // Use individual fields
                baseUrl = dataSource.ApiUrl ?? string.Empty;
                apiPath = dataSource.ApiPath;
                httpMethod = dataSource.HttpMethod ?? "GET";
                valuePath = dataSource.ValuePath ?? string.Empty;
                textPath = dataSource.TextPath ?? string.Empty;
                requestBody = !string.IsNullOrEmpty(requestBodyJson) 
                    ? requestBodyJson 
                    : dataSource.RequestBodyJson;
            }

            // Combine Base URL + Path to form full URL
            string fullApiUrl = CombineApiUrl(baseUrl, apiPath);
            _logger.LogInformation("API Request: Base URL: {BaseUrl}, Path: {Path}, Full URL: {FullUrl}", 
                baseUrl, apiPath ?? "(none)", fullApiUrl);

            // Use named HttpClient configured with automatic decompression
            var httpClient = _httpClientFactory.CreateClient("ExternalApi");
            httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout
            
            // Add browser-like headers to avoid Cloudflare challenges and 403 Forbidden errors
            // Use a real browser User-Agent to make requests look legitimate
            if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Accept"))
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            {
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Accept-Encoding"))
            {
                // Only request gzip and deflate - HttpClient handles these automatically
                // Brotli (br) may not be supported and can cause encoding issues
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Cache-Control"))
            {
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            }

            HttpResponseMessage response;
            string jsonContent;
            try
            {
                if (httpMethod.ToUpper() == "POST")
                {
                    var body = !string.IsNullOrEmpty(requestBody) 
                        ? JsonSerializer.Deserialize<JsonElement>(requestBody) 
                        : (JsonElement?)null;

                    response = await httpClient.PostAsJsonAsync(fullApiUrl, body);
                }
                else
                {
                    response = await httpClient.GetAsync(fullApiUrl);
                }

                // Check if response is successful
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var sanitizedError = SanitizeForJson(errorContent, 200); // Limit to 200 chars for error messages
                    _logger.LogWarning("API returned non-success status: {StatusCode} - {ReasonPhrase}. Response: {ErrorContent}", 
                        response.StatusCode, response.ReasonPhrase, sanitizedError);
                    throw new HttpRequestException($"API request failed with status {response.StatusCode} ({response.ReasonPhrase}). Response: {sanitizedError}");
                }

                // Read response content inside try-catch to handle encoding issues
                jsonContent = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling API: {FullUrl}", fullApiUrl);
                var sanitizedMessage = SanitizeForJson(ex.Message);
                throw new InvalidOperationException($"Failed to fetch data from API: {sanitizedMessage}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout when calling API: {FullUrl}", fullApiUrl);
                throw new InvalidOperationException($"API request timed out after 30 seconds: {fullApiUrl}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading response from API: {FullUrl}", fullApiUrl);
                var sanitizedMessage = SanitizeForJson(ex.Message);
                throw new InvalidOperationException($"Failed to read response from API: {sanitizedMessage}", ex);
            }
            
            // Check if response is HTML (Cloudflare challenge or error page)
            if (jsonContent.TrimStart().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
                jsonContent.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                jsonContent.Contains("cloudflare", StringComparison.OrdinalIgnoreCase) ||
                jsonContent.Contains("challenge", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("API returned HTML instead of JSON. This might be a Cloudflare challenge page. Response preview: {Preview}", 
                    jsonContent.Length > 500 ? jsonContent.Substring(0, 500) : jsonContent);
                throw new InvalidOperationException("API returned HTML page instead of JSON. This might be due to Cloudflare protection or the API endpoint is incorrect. Please check the API URL and try again.");
            }
            
            // Check if response might be compressed (starts with GZIP magic number 0x1F 0x8B)
            if (jsonContent.Length >= 2 && (byte)jsonContent[0] == 0x1F && (byte)jsonContent[1] == 0x8B)
            {
                _logger.LogError("API returned compressed (GZIP) response that was not decompressed. This indicates HttpClient automatic decompression is not working properly.");
                throw new InvalidOperationException("API returned compressed response that could not be decompressed. Please check HttpClient configuration.");
            }
            
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
                    
                    // 3. If no array path found, try common property names (supports any API structure)
                    if (!arrayElement.HasValue)
                    {
                        // Use custom array property names if provided by user, otherwise use default common names
                        var arrayPropertyNames = customArrayPropertyNames != null && customArrayPropertyNames.Any()
                            ? customArrayPropertyNames.ToList()
                            : new List<string> { 
                                "data", "results", "items", "list", "records", "values", "content", "collection",
                                "users", "products", "entries", "objects", "entities", "rows", "elements",
                                "array", "itemsList", "dataList", "resultList", "response", "payload", "body"
                            };
                        
                        _logger.LogInformation("Searching for array in properties: {Properties}", string.Join(", ", arrayPropertyNames));
                        
                        foreach (var propName in arrayPropertyNames)
                        {
                            // Try exact match first
                            if (root.TryGetProperty(propName, out var prop) && 
                                prop.ValueKind == JsonValueKind.Array && prop.GetArrayLength() > 0)
                            {
                                arrayElement = prop;
                                _logger.LogInformation("Found array at property: {PropertyName}", propName);
                                break;
                            }
                            
                            // Try case-insensitive match by enumerating properties
                            foreach (var jsonProp in root.EnumerateObject())
                            {
                                if (string.Equals(jsonProp.Name, propName, StringComparison.OrdinalIgnoreCase) &&
                                    jsonProp.Value.ValueKind == JsonValueKind.Array && jsonProp.Value.GetArrayLength() > 0)
                                {
                                    arrayElement = jsonProp.Value;
                                    _logger.LogInformation("Found array at property: {PropertyName} (case-insensitive match)", jsonProp.Name);
                                    break;
                                }
                            }
                            
                            if (arrayElement.HasValue)
                                break;
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
                _logger.LogError(ex, "Failed to parse JSON response from API: {FullUrl}. Error: {Error}", fullApiUrl, ex.Message);
                
                // Check if the error indicates a compression issue (0x1F is GZIP magic number)
                if (ex.Message.Contains("0x1F") || ex.Message.Contains("invalid start of a value"))
                {
                    var errorMsg = "API returned compressed response that could not be decompressed. " +
                                  "This usually means the response is GZIP-compressed but HttpClient automatic decompression is not enabled. " +
                                  "Please ensure HttpClient is configured with AutomaticDecompression enabled.";
                    _logger.LogError(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }
                
                var sanitizedMessage = SanitizeForJson(ex.Message);
                throw new InvalidOperationException($"Invalid JSON response from API: {sanitizedMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing API response from: {FullUrl}", fullApiUrl);
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
                var firstItem = arrayElement.EnumerateArray().First();
                var itemStructure = GetResponseStructure(firstItem, 2);
                var availableProps = string.Join(", ", firstItem.EnumerateObject().Select(p => p.Name));
                
                _logger.LogError("No options extracted! Check valuePath and textPath. " +
                    "valuePath: '{ValuePath}', textPath: '{TextPath}'. " +
                    "Available properties in first item: {Properties}. " +
                    "First item structure: {ItemStructure}", 
                    valuePath, textPath, availableProps, itemStructure);
                
                // Throw exception with helpful message
                throw new InvalidOperationException(
                    $"No options extracted from API response. " +
                    $"Please verify that valuePath '{valuePath}' and textPath '{textPath}' are correct. " +
                    $"Available properties in the first item: {availableProps}. " +
                    $"For nested properties, use dot notation (e.g., 'name.first' instead of 'first_name').");
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
        // INSPECT API STRUCTURE (Get Available Fields)
        // ================================
        public async Task<ApiResponse> InspectApiAsync(InspectApiRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ApiUrl))
                {
                    return new ApiResponse(400, "ApiUrl is required");
                }

                var response = new ApiInspectionResponseDto
                {
                    FullUrl = CombineApiUrl(request.ApiUrl, request.ApiPath),
                    Success = false
                };

                try
                {
                    // Use named HttpClient configured with automatic decompression
                    var httpClient = _httpClientFactory.CreateClient("ExternalApi");
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    
                    // Add browser-like headers
                    if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    }
                    if (!httpClient.DefaultRequestHeaders.Contains("Accept"))
                    {
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    }
                    if (!httpClient.DefaultRequestHeaders.Contains("Accept-Encoding"))
                    {
                        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    }

                    HttpResponseMessage httpResponse;
                    if ((request.HttpMethod ?? "GET").ToUpper() == "POST")
                    {
                        var body = !string.IsNullOrEmpty(request.RequestBodyJson) 
                            ? JsonSerializer.Deserialize<JsonElement>(request.RequestBodyJson) 
                            : (JsonElement?)null;
                        httpResponse = await httpClient.PostAsJsonAsync(response.FullUrl, body);
                    }
                    else
                    {
                        httpResponse = await httpClient.GetAsync(response.FullUrl);
                    }

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await httpResponse.Content.ReadAsStringAsync();
                        response.ErrorMessage = $"API returned status {httpResponse.StatusCode}: {errorContent.Substring(0, Math.Min(200, errorContent.Length))}";
                        return new ApiResponse(200, "API inspection completed", response);
                    }

                    var jsonContent = await httpResponse.Content.ReadAsStringAsync();
                    response.RawResponse = jsonContent.Length > 1000 ? jsonContent.Substring(0, 1000) + "..." : jsonContent;

                    var jsonDoc = JsonDocument.Parse(jsonContent);
                    var root = jsonDoc.RootElement;

                    // Find array in response
                    JsonElement? arrayElement = null;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        arrayElement = root;
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        var arrayPropertyNames = request.ArrayPropertyNames != null && request.ArrayPropertyNames.Any()
                            ? request.ArrayPropertyNames.ToList()
                            : new List<string> { 
                                "data", "results", "items", "list", "records", "values", "content", "collection",
                                "users", "products", "entries", "objects", "entities", "rows", "elements"
                            };
                        
                        foreach (var propName in arrayPropertyNames)
                        {
                            if (root.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.Array && prop.GetArrayLength() > 0)
                            {
                                arrayElement = prop;
                                break;
                            }
                            
                            foreach (var jsonProp in root.EnumerateObject())
                            {
                                if (string.Equals(jsonProp.Name, propName, StringComparison.OrdinalIgnoreCase) &&
                                    jsonProp.Value.ValueKind == JsonValueKind.Array && jsonProp.Value.GetArrayLength() > 0)
                                {
                                    arrayElement = jsonProp.Value;
                                    break;
                                }
                            }
                            
                            if (arrayElement.HasValue)
                                break;
                        }
                        
                        if (!arrayElement.HasValue)
                        {
                            arrayElement = FindFirstArray(root);
                        }
                    }

                    if (arrayElement.HasValue && arrayElement.Value.GetArrayLength() > 0)
                    {
                        var firstItem = arrayElement.Value[0];
                        response.ItemsCount = arrayElement.Value.GetArrayLength();
                        response.SampleItem = JsonSerializer.Deserialize<object>(firstItem.GetRawText());
                        
                        // Extract all available fields from first item
                        ExtractFields(firstItem, "", response.AvailableFields, response.NestedFields);
                        
                        // Suggest value paths (prefer id, key, code, value)
                        var valueSuggestions = new[] { "id", "key", "code", "value", "uuid", "identifier" };
                        foreach (var suggestion in valueSuggestions)
                        {
                            if (response.AvailableFields.Any(f => f.Equals(suggestion, StringComparison.OrdinalIgnoreCase)))
                            {
                                response.SuggestedValuePaths.Add(suggestion);
                            }
                        }
                        if (!response.SuggestedValuePaths.Any())
                        {
                            response.SuggestedValuePaths.AddRange(response.AvailableFields.Take(3));
                        }
                        
                        // Suggest text paths (prefer name, title, text, label)
                        var textSuggestions = new[] { "name", "title", "text", "label", "description", "firstname", "first_name" };
                        foreach (var suggestion in textSuggestions)
                        {
                            if (response.AvailableFields.Any(f => f.Equals(suggestion, StringComparison.OrdinalIgnoreCase)) ||
                                response.NestedFields.Any(f => f.Contains(suggestion, StringComparison.OrdinalIgnoreCase)))
                            {
                                var matchingField = response.AvailableFields.FirstOrDefault(f => f.Equals(suggestion, StringComparison.OrdinalIgnoreCase)) ??
                                                   response.NestedFields.FirstOrDefault(f => f.Contains(suggestion, StringComparison.OrdinalIgnoreCase));
                                if (matchingField != null)
                                {
                                    response.SuggestedTextPaths.Add(matchingField);
                                }
                            }
                        }
                        if (!response.SuggestedTextPaths.Any())
                        {
                            response.SuggestedTextPaths.AddRange(response.AvailableFields.Skip(1).Take(3));
                        }
                        
                        response.Success = true;
                    }
                    else
                    {
                        // No array found, show root structure
                        if (root.ValueKind == JsonValueKind.Object)
                        {
                            ExtractFields(root, "", response.AvailableFields, response.NestedFields);
                            response.SampleItem = JsonSerializer.Deserialize<object>(root.GetRawText());
                        }
                        response.ErrorMessage = "No array found in API response. Showing root structure instead.";
                        response.Success = false;
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = SanitizeForJson(ex.Message);
                    response.Success = false;
                }

                return new ApiResponse(200, "API inspection completed", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inspecting API structure");
                return new ApiResponse(500, $"Error inspecting API: {SanitizeForJson(ex.Message)}");
            }
        }

        private void ExtractFields(JsonElement element, string prefix, List<string> availableFields, List<string> nestedFields, int maxDepth = 3, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth || element.ValueKind != JsonValueKind.Object)
                return;

            foreach (var prop in element.EnumerateObject())
            {
                var fieldPath = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                
                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    nestedFields.Add(fieldPath);
                    ExtractFields(prop.Value, fieldPath, availableFields, nestedFields, maxDepth, currentDepth + 1);
                }
                else if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                {
                    nestedFields.Add(fieldPath);
                    if (prop.Value[0].ValueKind == JsonValueKind.Object)
                    {
                        ExtractFields(prop.Value[0], fieldPath, availableFields, nestedFields, maxDepth, currentDepth + 1);
                    }
                }
                else
                {
                    availableFields.Add(fieldPath);
                }
            }
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

                // Return array of table names (strings) for frontend compatibility
                var tableNames = suitableTables
                    .Select(t => ((dynamic)t).Name.ToString())
                    .OrderBy(name => name)
                    .ToList();

                return new ApiResponse(200, "Available lookup tables retrieved successfully", tableNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available lookup tables");
                return new ApiResponse(500, $"Error retrieving available lookup tables: {ex.Message}");
            }
        }

        // ================================
        // GET LOOKUP TABLE COLUMNS
        // ================================
        public async Task<ApiResponse> GetLookupTableColumnsAsync(string tableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    return new ApiResponse(400, "Table name is required");
                }

                // Validate table name to prevent SQL injection
                if (!IsValidIdentifier(tableName))
                {
                    return new ApiResponse(400, $"Invalid table name: {tableName}");
                }

                // Check if table exists using reflection
                var contextType = typeof(AkhmanageItContext);
                var dbSetProperty = contextType.GetProperty(tableName, System.Reflection.BindingFlags.IgnoreCase | 
                                                                        System.Reflection.BindingFlags.Public | 
                                                                        System.Reflection.BindingFlags.Instance);

                if (dbSetProperty == null)
                {
                    // Try case-insensitive search
                    var allProperties = contextType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    var matchingProperty = allProperties.FirstOrDefault(p => 
                        string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase) &&
                        p.PropertyType.IsGenericType && 
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
                    
                    if (matchingProperty == null)
                    {
                        var availableTables = string.Join(", ", allProperties
                            .Where(p => p.PropertyType.IsGenericType && 
                                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                            .Select(p => p.Name));
                        
                        _logger.LogWarning("Table '{TableName}' not found. Available tables: {AvailableTables}", 
                            tableName, availableTables);
                        return new ApiResponse(404, $"Table '{tableName}' not found. Available tables: {availableTables}");
                    }
                    
                    dbSetProperty = matchingProperty;
                }

                // Get the entity type
                var dbSetType = dbSetProperty.PropertyType;
                if (!dbSetType.IsGenericType || dbSetType.GetGenericTypeDefinition() != typeof(DbSet<>))
                {
                    return new ApiResponse(400, $"Property '{tableName}' is not a DbSet");
                }

                var entityType = dbSetType.GetGenericArguments()[0];

                // Get all properties (columns) from the entity type
                var columns = entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(p => !p.Name.StartsWith("_")) // Skip private fields
                    .Select(p => p.Name)
                    .OrderBy(name => name)
                    .ToList();

                if (columns.Count == 0)
                {
                    return new ApiResponse(404, $"No columns found for table '{tableName}'");
                }

                return new ApiResponse(200, $"Columns retrieved successfully for table '{tableName}'", columns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving columns for table '{TableName}'", tableName);
                return new ApiResponse(500, $"Error retrieving columns: {SanitizeForJson(ex.Message)}");
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
        // DELETE ALL OPTIONS FOR FIELD
        // ================================
        private async Task DeleteAllOptionsForFieldAsync(int fieldId)
        {
            try
            {
                var existingOptions = await _unitOfWork.FieldOptionsRepository.GetByFieldIdAsync(fieldId);
                foreach (var option in existingOptions)
                {
                    _unitOfWork.FieldOptionsRepository.Delete(option);
                }
                await _unitOfWork.CompleteAsyn();
                _logger.LogInformation("Deleted all options for field {FieldId} after setting Api/LookupTable DataSource", fieldId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting options for field {FieldId}", fieldId);
                // Don't throw - we still want to continue with DataSource creation/update
            }
        }

        // ================================
        // SAVE OPTIONS TO DATABASE (DEPRECATED - Only used for Static DataSource caching, not for Api/LookupTable)
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

        /// <summary>
        /// Combines Base URL and API Path to form full URL
        /// Supports flexible input: full URL or Base URL + Path
        /// Examples:
        /// - Base: "https://dummyjson.com/products" -> "https://dummyjson.com/products" (full URL, any random URL)
        /// - Base: "https://dummyjson.com/", Path: "products" -> "https://dummyjson.com/products"
        /// - Base: "https://randomuser.me/api/?results" -> "https://randomuser.me/api/?results" (full URL)
        /// - Base: "https://randomuser.me/api/", Path: "?results" -> "https://randomuser.me/api/?results"
        /// </summary>
        private string CombineApiUrl(string baseUrl, string? apiPath)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Base URL cannot be empty");
            }

            // If no path provided, use baseUrl as full URL (supports any random URL)
            // User can enter any URL directly in apiUrl field
            if (string.IsNullOrWhiteSpace(apiPath))
            {
                return baseUrl.Trim();
            }

            // If path is provided, combine Base URL + Path
            // Normalize: remove trailing slash from base, leading slash from path
            baseUrl = baseUrl.TrimEnd('/');
            apiPath = apiPath.TrimStart('/');

            // Combine: baseUrl + "/" + apiPath
            return $"{baseUrl}/{apiPath}";
        }
    }
}

