# 🚀 Hacker News Assessment — Monorepo Guide

🔗 **Live Demo:** [https://hacker-news-assessment.vercel.app/](https://hacker-news-assessment.vercel.app/)

This repo contains an **Angular 20 frontend** + **.NET 8 REST API** that implement a Hacker News reader with categories, search, paging, caching, and automated tests.

---

## 📂 Structure

- **`api/`** — .NET 8 Web API (DI, caching, typed `HttpClient`, Swagger, health check).  
- **`frontend/`** — Angular 20 SPA (Signals, router-driven categories, search, paging, Karma/Jasmine tests).

---

## ⚙️ Prerequisites

- **Node**: v20+  
- **.NET SDK**: v8  
- **Angular CLI** (optional): `npm i -g @angular/cli`  

---

## 🖥️ Setup & Run

### Backend — API
```bash
cd api
dotnet restore
dotnet run
```
Runs at **http://localhost:5021**  
✅ Health: `GET /api/health`  
✅ Swagger: `http://localhost:5021/swagger`

### Frontend — Angular
```bash
cd frontend
npm install
npm start
```
Runs at **http://localhost:4200**  
Connects to API at **http://localhost:5021**

---

## 🧪 Running Tests

**Frontend** (Karma + Jasmine):  
```bash
cd frontend
npm test
```

**Backend** (xUnit):  
```bash
cd api
dotnet test
```

---

## 🌐 API Endpoints

- `GET /api/stories?category=new&page=1&pageSize=30` → Paged stories  
- `GET /api/stories/search?category=new&q=term&page=1&pageSize=30` → Search stories  
- `GET /api/health` → `"Healthy!"`  

---

## ✨ Features

- 📰 **Homepage** (`/`) shows *newest* stories; added working tabs for `top`, `ask`, `show`, `jobs`, and hidden category `best`  
- 🔗 **Graceful link handling** (fallback to HN link if `url` missing)  
- 🔍 **Search per category** (searches cached stories if available, else fetches)  
- 📄 **Pagination** with “More” button + `hasMore` flag  
- 🗄️ **Caching** per category (60 seconds)  
- ⚡ **Parallel hydration** of stories (limit 200, 20 concurrent requests)  
- 🧩 **Interfaces/Contracts** consistently defined in both backend and frontend  
- 🧪 **Automated tests** (frontend specs + backend xUnit)  
- 🎨 **UX polish**: domain/timeAgo pipes, HN logo, dynamic titles  
- 📱 **Responsive layout** (mobile and desktop ready)  

---

## 🚀 Deployment Notes

- **Backend** → Deployable on Fly.io (CORS configured for Vercel).  
- **Frontend** → Deployable on Vercel (`vercel.json` handles SPA rewrites).  
- **Prod API URL**: `https://api-hacker-news-clone.fly.dev`

---

## ⚡ Scripts

Frontend `package.json`:
```json
{
  "scripts": {
    "start": "ng serve",
    "build": "ng build",
    "test": "ng test"
  }
}
```

---

## ✅ Above & Beyond

- Angular **Signals-first** design  
- **Unit-tested** fallback links  
- **Perf-friendly** API (cache + concurrency control)  
- **Responsive** front-end with routing tabs and dynamic categories  
- **Contracts & Interfaces** enforced in both backend and frontend  
- **Dev-friendly**: health endpoint, Swagger, environment configs  
