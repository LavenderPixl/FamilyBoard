import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import {useAuth} from "@/stores/auth.ts";
import LoginView from "@/views/LoginView.vue";

const router = createRouter({
  linkActiveClass: 'border-indigo-500',
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      meta: {
        requiresAuth: true,
      },
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
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      meta: {
        hideNavbar: true,
      },
      component: () => import('../views/NotFound.vue')
    }
  ],
})

// Routes user to login, if they aren't logged in/auth isn't valid and if view has "requiresAuth" meta.
router.beforeEach((to) => {
  const auth = useAuth()
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return {name: "login", component: LoginView}
  }
})
export default router
