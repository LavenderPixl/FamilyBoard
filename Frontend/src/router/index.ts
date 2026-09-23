import { createRouter, createWebHistory } from 'vue-router'
import LoginView from "@/views/LoginView.vue";
import SignupView  from "@/views/SignupView.vue";
import { authStore } from "@/stores/authStore.ts";
import { userStore } from "@/stores/userStore.ts";

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
      component: () => import('../views/HomeView.vue'),
    },
    {
      path: '/administrer-pligter',
      name: 'chore-administration',
      meta: {
        requiresAuth: true,
        requiresAdult: true,
      },
      component: () => import('../views/ChoreView.vue'),
    },
    {
      path: '/familieindstillinger',
      name: 'family-administration',
      meta: {
        requiresAuth: true,
        requiresAdult: true,
      },
      component: () => import('../views/FamilyView.vue'),
    },
    {
      path: '/kontoindstillinger',
      name: 'user-administration',
      meta: {
        requiresAuth: true,
      },
      component: () => import('../views/UserSettings.vue'),
    },
    {
      path:'/login',
      name: 'login',
      meta: {
        hideNavbar: true,
      },
      component: LoginView

    },{
      path:'/opret-bruger',
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
router.beforeEach(async (to) => {
  const auth = authStore()
  const user = userStore()

  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return { name: 'login' }
  }

  if (auth.isLoggedIn) {
    if (user.user) {
      auth.startRefTimer()
    } else {
      try {
        await user.getUser()
      } catch {
        auth.logout()
        return { name: 'login' }
      }
    }
  }

  if (to.meta.requiresAdult && !!user.user && !user.user.isAdult)
    return { name: 'home'}
})

// router.afterEach((to) => {
//   const auth = authStore()
//   if (auth.isLoggedIn) {
//     userStore().getUser()
//   }
// })
export default router
