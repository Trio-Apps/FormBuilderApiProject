/**
 * Client-side calculation engine for evaluating expressions
 */

/**
 * Extracts field codes from an expression (e.g., [RENT], [MONTHS])
 */
export function extractFieldCodes(expression: string): string[] {
  if (!expression) return []

  const pattern = /\[([A-Za-z_][A-Za-z0-9_]*)\]/g
  const matches = expression.matchAll(pattern)
  const fieldCodes: string[] = []

  for (const match of matches) {
    fieldCodes.push(match[1].toUpperCase())
  }

  return [...new Set(fieldCodes)] // Remove duplicates
}

/**
 * Processes advanced mathematical functions in the expression
 * Supports: SQRT(), ABS(), ROUND(), FLOOR(), CEIL(), MAX(), MIN(), SUM(), AVG(), POW(), MOD()
 */
function processAdvancedFunctions(expression: string): string {
  if (!expression) return expression

  let processed = expression

  // Handle Power operator: ^ or **
  processed = processed.replace(/(\d+(?:\.\d+)?)\s*\^\s*(\d+(?:\.\d+)?)/g, 
    (match, base, exp) => `Math.pow(${base}, ${exp})`)
  processed = processed.replace(/(\d+(?:\.\d+)?)\s*\*\*\s*(\d+(?:\.\d+)?)/g, 
    (match, base, exp) => `Math.pow(${base}, ${exp})`)

  // Handle SQRT(value) - Square Root
  processed = processed.replace(/SQRT\s*\(\s*([^)]+)\s*\)/gi, 
    (match, value) => `Math.sqrt(${value})`)

  // Handle ABS(value) - Absolute Value
  processed = processed.replace(/ABS\s*\(\s*([^)]+)\s*\)/gi, 
    (match, value) => `Math.abs(${value})`)

  // Handle ROUND(value) or ROUND(value, decimals)
  processed = processed.replace(/ROUND\s*\(\s*([^,)]+)(?:,\s*(\d+))?\s*\)/gi, 
    (match, value, decimals) => {
      const dec = decimals ? parseInt(decimals) : 0
      return `Math.round(${value} * Math.pow(10, ${dec})) / Math.pow(10, ${dec})`
    })

  // Handle FLOOR(value) - Round down
  processed = processed.replace(/FLOOR\s*\(\s*([^)]+)\s*\)/gi, 
    (match, value) => `Math.floor(${value})`)

  // Handle CEIL(value) or CEILING(value) - Round up
  processed = processed.replace(/CEIL(?:ING)?\s*\(\s*([^)]+)\s*\)/gi, 
    (match, value) => `Math.ceil(${value})`)

  // Handle MAX(value1, value2, ...) - Maximum value
  processed = processed.replace(/MAX\s*\(\s*([^)]+)\s*\)/gi, 
    (match, params) => {
      const values = params.split(',').map(v => v.trim())
      return `Math.max(${values.join(', ')})`
    })

  // Handle MIN(value1, value2, ...) - Minimum value
  processed = processed.replace(/MIN\s*\(\s*([^)]+)\s*\)/gi, 
    (match, params) => {
      const values = params.split(',').map(v => v.trim())
      return `Math.min(${values.join(', ')})`
    })

  // Handle SUM(value1, value2, ...) - Sum of values
  processed = processed.replace(/SUM\s*\(\s*([^)]+)\s*\)/gi, 
    (match, params) => {
      const values = params.split(',').map(v => v.trim())
      return `(${values.join(' + ')})`
    })

  // Handle AVG(value1, value2, ...) or AVERAGE(value1, value2, ...) - Average of values
  processed = processed.replace(/AVG(?:ERAGE)?\s*\(\s*([^)]+)\s*\)/gi, 
    (match, params) => {
      const values = params.split(',').map(v => v.trim())
      return `((${values.join(' + ')})) / ${values.length}`
    })

  // Handle POW(base, exponent) - Power function
  processed = processed.replace(/POW\s*\(\s*([^,)]+)\s*,\s*([^)]+)\s*\)/gi, 
    (match, base, exp) => `Math.pow(${base}, ${exp})`)

  // Handle MOD(value1, value2) - Modulo function
  processed = processed.replace(/MOD\s*\(\s*([^,)]+)\s*,\s*([^)]+)\s*\)/gi, 
    (match, val1, val2) => `(${val1}) % (${val2})`)

  return processed
}

/**
 * Evaluates an expression with provided field values
 */
export function evaluateExpression(
  expression: string,
  fieldValues: Record<string, any>,
  resultType: string = 'decimal'
): any {
  if (!expression) return null

  try {
    // Replace field codes with actual values
    let processedExpression = expression
    const fieldCodes = extractFieldCodes(expression)

    fieldCodes.forEach((fieldCode) => {
      // Try multiple variations: exact match, uppercase, lowercase
      const value = fieldValues[fieldCode] 
        ?? fieldValues[fieldCode.toUpperCase()] 
        ?? fieldValues[fieldCode.toLowerCase()] 
        ?? 0
      
      // Convert value to number string for calculation
      const numValue = parseFloat(String(value)) || 0
      
      // Replace both [FIELDCODE] and [fieldcode] variations (case-insensitive)
      processedExpression = processedExpression.replace(
        new RegExp(`\\[${fieldCode}\\]`, 'gi'),
        String(numValue)
      )
    })

    // Remove any remaining field codes (set to 0)
    processedExpression = processedExpression.replace(/\[[A-Za-z_][A-Za-z0-9_]*\]/gi, '0')

    // Process advanced mathematical functions
    processedExpression = processAdvancedFunctions(processedExpression)

    // Evaluate the expression using Function constructor (safer than eval)
    // Allow mathematical operations and Math functions
    // Updated regex to allow: numbers, operators, parentheses, Math., and whitespace
    const sanitizedExpression = processedExpression.replace(/[^0-9+\-*/().\s%Math]/gi, '')
    
    if (!sanitizedExpression) {
      return null
    }

    // Use Function constructor for safer evaluation
    // Create a safe context with Math object
    const result = new Function('Math', `return ${processedExpression}`)(Math)

    // Convert result based on result type
    switch (resultType?.toLowerCase()) {
      case 'integer':
      case 'int':
        return Math.round(Number(result))
      case 'decimal':
        return Number(result)
      case 'text':
      case 'string':
        return String(result)
      default:
        return Number(result)
    }
  } catch (error) {
    console.error('Error evaluating expression:', error)
    return null
  }
}

/**
 * Checks if an expression is valid (basic syntax check)
 */
export function isValidExpression(expression: string): boolean {
  if (!expression) return false

  try {
    // Extract field codes
    const fieldCodes = extractFieldCodes(expression)
    if (fieldCodes.length === 0) return false

    // Replace field codes with dummy values to test syntax
    let testExpression = expression
    fieldCodes.forEach((code) => {
      testExpression = testExpression.replace(new RegExp(`\\[${code}\\]`, 'gi'), '1')
    })

    // Remove any remaining field codes
    testExpression = testExpression.replace(/\[[A-Za-z_][A-Za-z0-9_]*\]/gi, '0')

    // Process advanced functions for validation
    testExpression = processAdvancedFunctions(testExpression)

    // Sanitize and test (allow Math functions)
    const sanitized = testExpression.replace(/[^0-9+\-*/().\s%Math]/gi, '')
    if (!sanitized) return false

    // Try to evaluate with Math context
    new Function('Math', `return ${testExpression}`)(Math)
    return true
  } catch {
    return false
  }
}

