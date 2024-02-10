import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import './index.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const root = document.getElementById('root');

const renderApp = () => {
    ReactDOM.createRoot(root).render(
        <React.StrictMode>
            <App />
        </React.StrictMode>
    );
};

if (import.meta.hot) {
    import.meta.hot.accept(['./App.jsx'], () => {
       
        renderApp();
    });
}


renderApp();
