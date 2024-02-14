import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
    plugins: [vue()],
    server: {
        proxy: {
            '/Converter': {
                target: 'http://localhost:5197',
                changeOrigin: true,
            },
            '/taskCompletedHub': {
                target: "http://localhost:5197",
                ws: true,
                secure: false,
                changeOrigin: true,
            }
        }
    }
})
