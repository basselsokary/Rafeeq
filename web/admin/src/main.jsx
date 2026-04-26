import { BrowserRouter } from 'react-router-dom'
import { createRoot } from 'react-dom/client'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'leaflet/dist/leaflet.css'
import "quill/dist/quill.snow.css";
import './styles/Map.css'
import './index.css'
import App from './App.jsx'

import L from 'leaflet';
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
  iconUrl: icon,
  shadowUrl: iconShadow,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34], 
});

L.Marker.prototype.options.icon = DefaultIcon;

createRoot(document.getElementById('root')).render(
  <BrowserRouter>
    < App />
  </BrowserRouter>

)
