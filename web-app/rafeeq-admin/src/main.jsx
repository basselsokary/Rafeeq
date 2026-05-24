import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { AuthProvider } from './context/AuthContext.jsx';
import { ThemeProvider } from './context/ThemeContext.jsx';
import { SidebarProvider } from './context/SidebarContext.jsx';
// import 'leaflet/dist/leaflet.css';

ReactDOM.createRoot(document.getElementById('root')).render(
  // <React.StrictMode>
    <ThemeProvider>
      <AuthProvider>
        <SidebarProvider>
          <App />
        </SidebarProvider>
      </AuthProvider>
    </ThemeProvider>
  // </React.StrictMode>
    // <App />
);