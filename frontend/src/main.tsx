import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import App from './App'
import './index.css'
import './styles/rtl.css'
import './i18n/config'
import { LanguageProvider } from './contexts/LanguageContext'

// Suppress errors from browser extensions (like translate-page, save-page)
window.addEventListener('error', (event) => {
  if (event.message && (
    event.message.includes('translate-page') || 
    event.message.includes('save-page') ||
    event.message.includes('Cannot find menu item')
  )) {
    event.preventDefault()
    return false
  }
})

// Suppress unhandled promise rejections from browser extensions
window.addEventListener('unhandledrejection', (event) => {
  if (event.reason) {
    const message = typeof event.reason === 'string' 
      ? event.reason 
      : (typeof event.reason === 'object' && 'message' in event.reason)
        ? String(event.reason.message)
        : String(event.reason)
    
    if (message.includes('translate-page') || 
        message.includes('save-page') ||
        message.includes('Cannot find menu item')) {
      event.preventDefault()
      return false
    }
  }
})

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <LanguageProvider>
        <App />
      </LanguageProvider>
    </BrowserRouter>
  </React.StrictMode>,
)












