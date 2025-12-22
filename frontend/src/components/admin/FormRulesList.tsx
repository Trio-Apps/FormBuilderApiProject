import { useState, useEffect } from 'react'
import FormRuleBuilder from './FormRuleBuilder'
import './FormRulesList.css'

interface FormRule {
  id: number
  ruleName: string
  condition: any
  actions: any[]
  elseActions?: any[]
  isActive: boolean
  executionOrder: number
}

interface FormRulesListProps {
  formBuilderId: number
  fields: Array<{ id: number; fieldCode: string; fieldName: string }>
}

const FormRulesList = ({ formBuilderId, fields }: FormRulesListProps) => {
  const [rules, setRules] = useState<FormRule[]>([])
  const [loading, setLoading] = useState(true)
  const [showBuilder, setShowBuilder] = useState(false)
  const [editingRule, setEditingRule] = useState<FormRule | null>(null)

  useEffect(() => {
    loadRules()
  }, [formBuilderId])

  const loadRules = async () => {
    try {
      setLoading(true)
      const response = await fetch(`/api/FormRules/form/${formBuilderId}`)
      if (!response.ok) throw new Error('Failed to load rules')
      const data = await response.json()
      setRules(data)
    } catch (error) {
      console.error('Error loading rules:', error)
      alert('Failed to load rules')
    } finally {
      setLoading(false)
    }
  }

  const handleSaveRule = async (rule: FormRule) => {
    try {
      const url = rule.id 
        ? `/api/FormRules/${rule.id}` 
        : '/api/FormRules'
      
      const method = rule.id ? 'PUT' : 'POST'

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          formBuilderId,
          ruleName: rule.ruleName,
          ruleJson: JSON.stringify({
            condition: rule.condition,
            actions: rule.actions,
            elseActions: rule.elseActions
          }),
          isActive: rule.isActive,
          executionOrder: rule.executionOrder
        })
      })

      if (!response.ok) throw new Error('Failed to save rule')

      await loadRules()
      setShowBuilder(false)
      setEditingRule(null)
    } catch (error) {
      console.error('Error saving rule:', error)
      alert('Failed to save rule')
    }
  }

  const handleDeleteRule = async (id: number) => {
    if (!confirm('Are you sure you want to delete this rule?')) return

    try {
      const response = await fetch(`/api/FormRules/${id}`, {
        method: 'DELETE'
      })

      if (!response.ok) throw new Error('Failed to delete rule')

      await loadRules()
    } catch (error) {
      console.error('Error deleting rule:', error)
      alert('Failed to delete rule')
    }
  }

  const handleToggleActive = async (rule: FormRule) => {
    try {
      const response = await fetch(`/api/FormRules/${rule.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          formBuilderId: rule.id,
          ruleName: rule.ruleName,
          ruleJson: JSON.stringify({
            condition: rule.condition,
            actions: rule.actions,
            elseActions: rule.elseActions
          }),
          isActive: !rule.isActive,
          executionOrder: rule.executionOrder
        })
      })

      if (!response.ok) throw new Error('Failed to update rule')

      await loadRules()
    } catch (error) {
      console.error('Error updating rule:', error)
      alert('Failed to update rule')
    }
  }

  const handleEdit = (rule: FormRule) => {
    setEditingRule(rule)
    setShowBuilder(true)
  }

  const handleNewRule = () => {
    setEditingRule(null)
    setShowBuilder(true)
  }

  const handleCancel = () => {
    setShowBuilder(false)
    setEditingRule(null)
  }

  if (showBuilder) {
    return (
      <FormRuleBuilder
        formBuilderId={formBuilderId}
        fields={fields}
        initialRule={editingRule || undefined}
        onSave={handleSaveRule}
        onCancel={handleCancel}
      />
    )
  }

  if (loading) {
    return <div className="loading">Loading rules...</div>
  }

  const sortedRules = [...rules].sort((a, b) => a.executionOrder - b.executionOrder)

  return (
    <div className="form-rules-list">
      <div className="list-header">
        <h2>Form Rules</h2>
        <button onClick={handleNewRule} className="btn-primary">
          + Create New Rule
        </button>
      </div>

      {sortedRules.length === 0 ? (
        <div className="empty-state">
          <p>No rules defined yet. Create your first rule to get started.</p>
        </div>
      ) : (
        <div className="rules-grid">
          {sortedRules.map(rule => (
            <div key={rule.id} className={`rule-card ${!rule.isActive ? 'inactive' : ''}`}>
              <div className="rule-header">
                <div className="rule-title">
                  <h3>{rule.ruleName}</h3>
                  <span className="order-badge">Order: {rule.executionOrder}</span>
                </div>
                <div className="rule-status">
                  <label>
                    <input
                      type="checkbox"
                      checked={rule.isActive}
                      onChange={() => handleToggleActive(rule)}
                    />
                    Active
                  </label>
                </div>
              </div>

              <div className="rule-body">
                <div className="rule-section">
                  <strong>Condition:</strong>
                  <div className="condition-display">
                    IF {rule.condition?.field} {rule.condition?.operator} {rule.condition?.value}
                  </div>
                </div>

                <div className="rule-section">
                  <strong>Actions ({rule.actions?.length || 0}):</strong>
                  <ul>
                    {rule.actions?.slice(0, 3).map((action, idx) => (
                      <li key={idx}>
                        {action.type}({action.fieldCode}
                        {action.value !== undefined && `, ${action.value}`}
                        {action.expression && `, ${action.expression}`})
                      </li>
                    ))}
                    {rule.actions?.length > 3 && (
                      <li>... and {rule.actions.length - 3} more</li>
                    )}
                  </ul>
                </div>

                {rule.elseActions && rule.elseActions.length > 0 && (
                  <div className="rule-section">
                    <strong>Else Actions ({rule.elseActions.length}):</strong>
                    <ul>
                      {rule.elseActions.slice(0, 2).map((action, idx) => (
                        <li key={idx}>
                          {action.type}({action.fieldCode})
                        </li>
                      ))}
                      {rule.elseActions.length > 2 && (
                        <li>... and {rule.elseActions.length - 2} more</li>
                      )}
                    </ul>
                  </div>
                )}
              </div>

              <div className="rule-actions">
                <button onClick={() => handleEdit(rule)} className="btn-edit">
                  Edit
                </button>
                <button
                  onClick={() => handleDeleteRule(rule.id)}
                  className="btn-delete"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default FormRulesList

