// frontend/vite.config.ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import fs from 'fs'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    host: '0.0.0.0',
    port: 5173,
    https: {
      // Загружаем PFX напрямую
      pfx: fs.readFileSync(path.resolve(__dirname, 'certs/aspnetapp.pfx')),
      passphrase: process.env.CERT_PASSWORD
    },
    proxy: {
      '/api': {
        target: 'https://messenger-api:443',
        changeOrigin: true,
        secure: false
      },
      '/hubs': {
        target: 'https://messenger-api:443',    // ← было wss://messenger-api:443
        ws: true,
        changeOrigin: true,
        secure: false
      }
    }
  }
})
