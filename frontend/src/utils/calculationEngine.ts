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

    // Evaluate the expression using Function constructor (safer than eval)
    // Only allow mathematical operations
    const sanitizedExpression = processedExpression.replace(/[^0-9+\-*/().\s]/g, '')
    
    if (!sanitizedExpression) {
      return null
    }

    // Use Function constructor for safer evaluation
    const result = new Function(`return ${sanitizedExpression}`)()

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

    // Sanitize and test
    const sanitized = testExpression.replace(/[^0-9+\-*/().\s]/g, '')
    if (!sanitized) return false

    // Try to evaluate
    new Function(`return ${sanitized}`)()
    return true
  } catch {
    return false
  }
}

