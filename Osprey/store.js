import {create} from 'zustand';

const useAuthStore = create((set) => ({
  user: null, // Stores user data (id, role, email, etc.)
  login: (userData) => set({ user: userData }), // Action to log in
  logout: () => set({ user: null }), // Action to log out
}));

export default useAuthStore;