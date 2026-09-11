import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from "../views/LoginView.vue";
import SignupView  from "../views/SignupView.vue";
import { authStore } from "../stores/authStore.ts";
import { userStore } from "../stores/userStore.ts";

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
      component: LoginView

    },{
      path:'/signup',
      name: 'signup',
      meta: {
        hideNavbar: true,
      },
      component: SignupView

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
  const auth = authStore()
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return {name: "login", component: LoginView}
  }
})

router.afterEach((to) => {
  const auth = authStore()
  if (auth.isLoggedIn) {
    userStore().getUser()
  }
})
export default router
