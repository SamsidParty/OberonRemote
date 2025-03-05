import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import "./SocketClient.jsx"
import "./GamepadInput.jsx"
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
