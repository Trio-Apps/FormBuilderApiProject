import { Routes, Route } from 'react-router-dom'
import FormViewer from './pages/FormViewer'
import FormBuilderAdmin from './pages/FormBuilderAdmin'

function App() {
  return (
    <Routes>
      <Route path="/forms/view/:formPublicId" element={<FormViewer />} />
      <Route path="/admin/form-builder/:id" element={<FormBuilderAdmin />} />
      <Route path="*" element={<div>Page not found</div>} />
    </Routes>
  )
}

export default App












