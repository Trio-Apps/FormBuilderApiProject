import { Routes, Route } from 'react-router-dom'
import FormViewer from './pages/FormViewer'

function App() {
  return (
    <Routes>
      <Route path="/forms/view/:formPublicId" element={<FormViewer />} />
      <Route path="*" element={<div>Page not found</div>} />
    </Routes>
  )
}

export default App







