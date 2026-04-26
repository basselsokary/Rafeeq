import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

export default defineConfig({
  plugins: [react()],
  optimizeDeps: {
    include: ['react-quilljs', 'quill'],
  },
  define: {
    'global': 'window',
  },
  build: {
    commonjsOptions: {
      transformMixedEsModules: true, 
    },
  },
})