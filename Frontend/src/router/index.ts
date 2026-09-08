import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  linkActiveClass: 'border-indigo-500',
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path:'/login',
      name: 'login',
      meta: {
        hideNavbar: true,
      },
      component: () => import('../views/LoginView.vue'),

    },{
      path:'/signup',
      name: 'signup',
      meta: {
        hideNavbar: true,
      },
      component: () => import('../views/SignupView.vue'),

    },
    // {
    //   path: '/:pathMatch(.*)*',
    //   name: 'NotFound',
    //   component: () => import('../views/NotFound.vue')
    // }
  ],
})

export default router
