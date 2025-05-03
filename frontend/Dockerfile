# frontend/Dockerfile
FROM node:20

WORKDIR /app

# 1. Копируем и устанавливаем зависимости
COPY package*.json ./
RUN npm ci

# 2. Копируем исходники и сертификаты
COPY . .
# (в том числе папку certs)

# 3. Открываем порт Vite
EXPOSE 5173

# 4. Запускаем dev-сервер
CMD ["npm", "run", "dev"]
