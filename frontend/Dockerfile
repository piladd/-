# Этап сборки (builder)
FROM node:20-alpine AS builder

WORKDIR /app

# Устанавливаем зависимости
COPY package*.json ./
RUN npm install

# Копируем остальные файлы и собираем проект
COPY . .
RUN npm run build

# Этап продакшен-сервера (Nginx)
FROM nginx:alpine

# Копируем собранные файлы из builder-этапа
COPY --from=builder /app/dist /usr/share/nginx/html

# (необязательно) Кастомная конфигурация nginx — если понадобится
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
