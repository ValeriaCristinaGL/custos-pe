# Frontend Dockerfile exemplo
FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .

# Create dist directory with index.html if npm run build doesn't exist
RUN npm run build || (mkdir -p dist && cp index.html dist/index.html)

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
