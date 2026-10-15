import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    // Encaminha /api para a API .NET (http://localhost:5120).
    // Assim o navegador enxerga tudo na mesma origem e não há CORS no dev.
    proxy: {
      '/api': {
        target: 'http://localhost:5120',
        changeOrigin: true,
      },
    },
  },
})
