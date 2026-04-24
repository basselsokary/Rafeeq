import React from 'react'
import { Routes, Route } from 'react-router-dom'
import Dashboard from './pages/dashboard/Dashboard'
import CitiesList from './pages/cities/CitiesList'
import CitiesEditor from './pages/cities/CitiesEditor'
import { ThemeProvider } from './components/Theme'

function App() {

  return (
    <ThemeProvider>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/cities" element={<CitiesList />} />
        <Route path="/cityEditor" element={<CitiesEditor />} />
      </Routes>
    </ThemeProvider>
  )
}

export default App
