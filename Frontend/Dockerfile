FROM node:lts-alpine

RUN npm install -g http-server

WORKDIR /app
COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

EXPOSE 8080

CMD ["http-server", "dist", "--proxy", "http://127.0.0.1:8080?"]