/** @type {import('tailwindcss').Config} */
export default {
  darkMode: ['class'],
  content: [
    './frontend/src/**/*.{ts,tsx}',
  ],
  theme: {
    extend: {
      // Design tokens will be defined here
      colors: {
        // Primary colors - to be customized based on brand
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
          950: '#172554',
        },
        // Secondary colors
        secondary: {
          50: '#f8fafc',
          100: '#f1f5f9',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#273549',
          900: '#1e293b',
          950: '#0f172a',
        },
      },
      spacing: {
        // Custom spacing values if needed
      },
      borderRadius: {
        // Custom border radius values
        'sm': '0.125rem',
        'DEFAULT': '0.25rem',
        'md': '0.375rem',
        'lg': '0.5rem',
        'xl': '0.75rem',
        '2xl': '1rem',
        '3xl': '1.5rem',
      },
    },
  },
  plugins: [],
};
