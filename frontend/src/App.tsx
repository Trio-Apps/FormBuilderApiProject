import { Routes, Route } from 'react-router-dom'
import FormViewer from './pages/FormViewer'
import FormBuilderAdmin from './pages/FormBuilderAdmin'
import DocumentTypesPage from './pages/DocumentTypes'

function App() {
  return (
    <Routes>
      <Route path="/forms/view/:formPublicId" element={<FormViewer />} />
      <Route path="/admin/form-builder/:id" element={<FormBuilderAdmin />} />
      <Route path="/admin/document-types" element={<DocumentTypesPage />} />
      <Route path="*" element={<div>Page not found</div>} />
    </Routes>
  )
}

export default App












