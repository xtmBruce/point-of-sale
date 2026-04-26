/**
 * Application configuration
 * Centralized configuration management using environment variables
 */

// Detect if we're running on the remote server
const isRemoteServer = typeof window !== 'undefined' &&
  (window.location.hostname === '84.247.131.178' ||
    window.location.hostname.includes('84.247.131.178') ||
    window.location.hostname === 'likaships.com' ||
    window.location.hostname.includes('likaships.com'));

const config = {
  // API Configuration
  api: {
    // Use environment variable for production, fallback to localhost for development
    baseURL: (import.meta.env.PROD || isRemoteServer)
      ? (import.meta.env.VITE_API_URL || 'https://smartpos-retail.onrender.com/api')  // Production: use env var or default SmartPOS backend
      : (import.meta.env.VITE_API_URL || 'https://localhost:7086/api'),  // Development: use env var or local API
    timeout: 30000, // 30 seconds
  },

  // Environment
  env: {
    isDevelopment: import.meta.env.DEV,
    isProduction: import.meta.env.PROD,
    mode: import.meta.env.MODE || 'development',
  },

  // App Configuration
  app: {
    name: import.meta.env.VITE_APP_NAME || 'Likaperfumes',
    version: import.meta.env.VITE_APP_VERSION || '1.0.0',
  },

  // Feature flags (optional)
  features: {
    enableAnalytics: import.meta.env.VITE_ENABLE_ANALYTICS === 'true',
    enableDebugMode: import.meta.env.VITE_DEBUG_MODE === 'true',
  }
}

// Log configuration in development or when on remote server
if (config.env.isDevelopment || isRemoteServer) {
  console.log('🔧 App Configuration:', {
    api: config.api.baseURL,
    environment: config.env.mode,
    isRemoteServer: isRemoteServer,
    hostname: typeof window !== 'undefined' ? window.location.hostname : 'N/A',
    app: config.app.name,
    version: config.app.version,
  })
}

export default config
