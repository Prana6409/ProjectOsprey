// utils/axios.js
import axios from 'axios';

// Set up the base URL for the API
const API_URL = 'https://localhost:7122'; // Update with your backend URL

const axiosInstance = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export default axiosInstance;
