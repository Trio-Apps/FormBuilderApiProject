import { useState } from 'react'
import { Condition, Action, FormRule } from '../../types/formRules'
import './FormRuleBuilder.css'

interface FormRuleBuilderProps {
  formBuilderId: number
  fields: Array<{ id: number; fieldCode: string; fieldName: string }>
  initialRule?: FormRule
  onSave: (rule: FormRule) => Promise<void>
  onCancel: () => void
}

const FormRuleBuilder = ({
  formBuilderId,
  fields,
  initialRule,
  onSave,
  onCancel
}: FormRuleBuilderProps) => {
  const [ruleName, setRuleName] = useState(initialRule?.ruleName || '')
  const [condition, setCondition] = useState<Condition>(
    initialRule?.condition || {
      field: '',
      operator: '==',
      value: '',
      valueType: 'constant'
    }
  )
  const [actions, setActions] = useState<Action[]>(initialRule?.actions || [])
  const [elseActions, setElseActions] = useState<Action[]>(initialRule?.elseActions || [])
  const [isActive, setIsActive] = useState(initialRule?.isActive ?? true)
  const [executionOrder, setExecutionOrder] = useState(initialRule?.executionOrder || 1)
  const [showActionDialog, setShowActionDialog] = useState(false)
  const [showElseActionDialog, setShowElseActionDialog] = useState(false)
  const [newAction, setNewAction] = useState<Partial<Action>>({})

  const operators = [
    { value: '==', label: 'Equals (==)' },
    { value: '!=', label: 'Not Equals (!=)' },
    { value: '>', label: 'Greater Than (>)' },
    { value: '<', label: 'Less Than (<)' },
    { value: '>=', label: 'Greater or Equal (>=)' },
    { value: '<=', label: 'Less or Equal (<=)' },
    { value: 'contains', label: 'Contains' },
    { value: 'isEmpty', label: 'Is Empty' },
    { value: 'isNotEmpty', label: 'Is Not Empty' }
  ]

  const actionTypes = [
    { value: 'SetVisible', label: 'Set Visible' },
    { value: 'SetReadOnly', label: 'Set Read Only' },
    { value: 'SetMandatory', label: 'Set Mandatory' },
    { value: 'SetDefault', label: 'Set Default Value' },
    { value: 'ClearValue', label: 'Clear Value' },
    { value: 'Compute', label: 'Compute (Formula)' }
  ]

  const handleAddAction = () => {
    if (!newAction.type || !newAction.fieldCode) {
      alert('Please fill all required fields')
      return
    }

    const action: Action = {
      type: newAction.type!,
      fieldCode: newAction.fieldCode!,
      value: newAction.value,
      expression: newAction.expression
    }

    setActions([...actions, action])
    setNewAction({})
    setShowActionDialog(false)
  }

  const handleAddElseAction = () => {
    if (!newAction.type || !newAction.fieldCode) {
      alert('Please fill all required fields')
      return
    }

    const action: Action = {
      type: newAction.type!,
      fieldCode: newAction.fieldCode!,
      value: newAction.value,
      expression: newAction.expression
    }

    setElseActions([...elseActions, action])
    setNewAction({})
    setShowElseActionDialog(false)
  }

  const handleRemoveAction = (index: number) => {
    setActions(actions.filter((_, i) => i !== index))
  }

  const handleRemoveElseAction = (index: number) => {
    setElseActions(elseActions.filter((_, i) => i !== index))
  }

  const handleSave = async () => {
    if (!ruleName.trim()) {
      alert('Rule name is required')
      return
    }

    if (!condition.field) {
      alert('Please select a field for the condition')
      return
    }

    if (actions.length === 0) {
      alert('Please add at least one action')
      return
    }

    const rule: FormRule = {
      id: initialRule?.id,
      ruleName: ruleName.trim(),
      condition,
      actions,
      elseActions: elseActions.length > 0 ? elseActions : undefined,
      isActive,
      executionOrder
    }

    await onSave(rule)
  }

  const getActionLabel = (action: Action) => {
    const field = fields.find(f => f.fieldCode === action.fieldCode)
    const fieldName = field?.fieldName || action.fieldCode
    const actionTypeLabel = actionTypes.find(t => t.value === action.type)?.label || action.type

    switch (action.type) {
      case 'SetVisible':
      case 'SetReadOnly':
      case 'SetMandatory':
        return `${actionTypeLabel}(${fieldName}, ${action.value ? 'true' : 'false'})`
      case 'SetDefault':
        return `${actionTypeLabel}(${fieldName}, ${action.value})`
      case 'ClearValue':
        return `${actionTypeLabel}(${fieldName})`
      case 'Compute':
        return `${actionTypeLabel}(${fieldName}, ${action.expression})`
      default:
        return `${action.type}(${fieldName})`
    }
  }

  return (
    <div className="form-rule-builder">
      <div className="builder-header">
        <h3>{initialRule ? 'Edit Rule' : 'Create New Rule'}</h3>
      </div>

      <div className="builder-section">
        <label>Rule Name *</label>
        <input
          type="text"
          value={ruleName}
          onChange={(e) => setRuleName(e.target.value)}
          placeholder="Show Parking Details"
          className="full-width"
        />
      </div>

      <div className="builder-section">
        <label>Execution Order</label>
        <input
          type="number"
          value={executionOrder}
          onChange={(e) => setExecutionOrder(parseInt(e.target.value) || 1)}
          min="1"
          className="order-input"
        />
        <small>Lower numbers execute first</small>
      </div>

      <div className="builder-section">
        <h4>Condition</h4>
        <div className="condition-builder">
          <div className="condition-row">
            <label>Field</label>
            <select
              value={condition.field}
              onChange={(e) => setCondition({ ...condition, field: e.target.value })}
            >
              <option value="">Select Field</option>
              {fields.map(field => (
                <option key={field.id} value={field.fieldCode}>
                  {field.fieldName} ({field.fieldCode})
                </option>
              ))}
            </select>
          </div>

          <div className="condition-row">
            <label>Operator</label>
            <select
              value={condition.operator}
              onChange={(e) => setCondition({ ...condition, operator: e.target.value })}
            >
              {operators.map(op => (
                <option key={op.value} value={op.value}>
                  {op.label}
                </option>
              ))}
            </select>
          </div>

          {condition.operator !== 'isEmpty' && condition.operator !== 'isNotEmpty' && (
            <div className="condition-row">
              <label>Value</label>
              <div className="value-type-selector">
                <label>
                  <input
                    type="radio"
                    checked={condition.valueType === 'constant'}
                    onChange={() => setCondition({ ...condition, valueType: 'constant' })}
                  />
                  Constant
                </label>
                <label>
                  <input
                    type="radio"
                    checked={condition.valueType === 'field'}
                    onChange={() => setCondition({ ...condition, valueType: 'field' })}
                  />
                  Another Field
                </label>
              </div>

              {condition.valueType === 'constant' ? (
                <input
                  type="text"
                  value={condition.value}
                  onChange={(e) => setCondition({ ...condition, value: e.target.value })}
                  placeholder="Enter value"
                  className="full-width"
                />
              ) : (
                <select
                  value={condition.value}
                  onChange={(e) => setCondition({ ...condition, value: e.target.value })}
                >
                  <option value="">Select Field</option>
                  {fields.map(field => (
                    <option key={field.id} value={field.fieldCode}>
                      {field.fieldName} ({field.fieldCode})
                    </option>
                  ))}
                </select>
              )}
            </div>
          )}
        </div>
      </div>

      <div className="builder-section">
        <h4>Actions (When Condition is True)</h4>
        <div className="actions-list">
          {actions.map((action, index) => (
            <div key={index} className="action-item">
              <span>{getActionLabel(action)}</span>
              <button
                type="button"
                onClick={() => handleRemoveAction(index)}
                className="btn-remove"
              >
                Remove
              </button>
            </div>
          ))}
          {actions.length === 0 && (
            <p className="empty-message">No actions added yet</p>
          )}
        </div>
        <button
          type="button"
          onClick={() => setShowActionDialog(true)}
          className="btn-add"
        >
          + Add Action
        </button>
      </div>

      <div className="builder-section">
        <h4>Else Actions (When Condition is False) - Optional</h4>
        <div className="actions-list">
          {elseActions.map((action, index) => (
            <div key={index} className="action-item">
              <span>{getActionLabel(action)}</span>
              <button
                type="button"
                onClick={() => handleRemoveElseAction(index)}
                className="btn-remove"
              >
                Remove
              </button>
            </div>
          ))}
          {elseActions.length === 0 && (
            <p className="empty-message">No else actions added</p>
          )}
        </div>
        <button
          type="button"
          onClick={() => setShowElseActionDialog(true)}
          className="btn-add"
        >
          + Add Else Action
        </button>
      </div>

      <div className="builder-section">
        <label>
          <input
            type="checkbox"
            checked={isActive}
            onChange={(e) => setIsActive(e.target.checked)}
          />
          Is Active
        </label>
      </div>

      <div className="builder-actions">
        <button
          type="button"
          onClick={handleSave}
          className="btn-primary"
        >
          Save Rule
        </button>
        <button
          type="button"
          onClick={onCancel}
          className="btn-secondary"
        >
          Cancel
        </button>
      </div>

      {/* Action Dialog */}
      {showActionDialog && (
        <div className="dialog-overlay" onClick={() => setShowActionDialog(false)}>
          <div className="dialog-content" onClick={(e) => e.stopPropagation()}>
            <h4>Add Action</h4>
            <div className="dialog-section">
              <label>Action Type *</label>
              <select
                value={newAction.type || ''}
                onChange={(e) => setNewAction({ ...newAction, type: e.target.value })}
              >
                <option value="">Select Action Type</option>
                {actionTypes.map(type => (
                  <option key={type.value} value={type.value}>
                    {type.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="dialog-section">
              <label>Target Field *</label>
              <select
                value={newAction.fieldCode || ''}
                onChange={(e) => setNewAction({ ...newAction, fieldCode: e.target.value })}
              >
                <option value="">Select Field</option>
                {fields.map(field => (
                  <option key={field.id} value={field.fieldCode}>
                    {field.fieldName} ({field.fieldCode})
                  </option>
                ))}
              </select>
            </div>

            {(newAction.type === 'SetVisible' || 
              newAction.type === 'SetReadOnly' || 
              newAction.type === 'SetMandatory') && (
              <div className="dialog-section">
                <label>Value</label>
                <select
                  value={newAction.value !== undefined ? String(newAction.value) : ''}
                  onChange={(e) => setNewAction({ ...newAction, value: e.target.value === 'true' })}
                >
                  <option value="true">True</option>
                  <option value="false">False</option>
                </select>
              </div>
            )}

            {newAction.type === 'SetDefault' && (
              <div className="dialog-section">
                <label>Default Value</label>
                <input
                  type="text"
                  value={newAction.value || ''}
                  onChange={(e) => setNewAction({ ...newAction, value: e.target.value })}
                  placeholder="Enter default value"
                />
              </div>
            )}

            {newAction.type === 'Compute' && (
              <div className="dialog-section">
                <label>Expression</label>
                <input
                  type="text"
                  value={newAction.expression || ''}
                  onChange={(e) => setNewAction({ ...newAction, expression: e.target.value })}
                  placeholder="price * quantity"
                />
                <small>Use field codes as variables (e.g., price * quantity)</small>
              </div>
            )}

            <div className="dialog-actions">
              <button
                type="button"
                onClick={handleAddAction}
                className="btn-primary"
              >
                Add
              </button>
              <button
                type="button"
                onClick={() => {
                  setShowActionDialog(false)
                  setNewAction({})
                }}
                className="btn-secondary"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Else Action Dialog */}
      {showElseActionDialog && (
        <div className="dialog-overlay" onClick={() => setShowElseActionDialog(false)}>
          <div className="dialog-content" onClick={(e) => e.stopPropagation()}>
            <h4>Add Else Action</h4>
            {/* Same content as Action Dialog */}
            <div className="dialog-section">
              <label>Action Type *</label>
              <select
                value={newAction.type || ''}
                onChange={(e) => setNewAction({ ...newAction, type: e.target.value })}
              >
                <option value="">Select Action Type</option>
                {actionTypes.map(type => (
                  <option key={type.value} value={type.value}>
                    {type.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="dialog-section">
              <label>Target Field *</label>
              <select
                value={newAction.fieldCode || ''}
                onChange={(e) => setNewAction({ ...newAction, fieldCode: e.target.value })}
              >
                <option value="">Select Field</option>
                {fields.map(field => (
                  <option key={field.id} value={field.fieldCode}>
                    {field.fieldName} ({field.fieldCode})
                  </option>
                ))}
              </select>
            </div>

            {(newAction.type === 'SetVisible' || 
              newAction.type === 'SetReadOnly' || 
              newAction.type === 'SetMandatory') && (
              <div className="dialog-section">
                <label>Value</label>
                <select
                  value={newAction.value !== undefined ? String(newAction.value) : ''}
                  onChange={(e) => setNewAction({ ...newAction, value: e.target.value === 'true' })}
                >
                  <option value="true">True</option>
                  <option value="false">False</option>
                </select>
              </div>
            )}

            <div className="dialog-actions">
              <button
                type="button"
                onClick={handleAddElseAction}
                className="btn-primary"
              >
                Add
              </button>
              <button
                type="button"
                onClick={() => {
                  setShowElseActionDialog(false)
                  setNewAction({})
                }}
                className="btn-secondary"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default FormRuleBuilder

