import { Alert, Pressable, StyleSheet, Text, View } from 'react-native';
import React, { useRef, useState } from 'react';
import ScreenWrapper from '../components/ScreenWrapper';
import { theme } from '../constants/theme';
import { StatusBar } from 'expo-status-bar';
import { hp, wp } from '../helpers/common';
import Input from '../components/input';
import { useRouter } from 'expo-router';
import Button from '../components/Button';
import BackButton from '../components/BackButton';
import Icon from '../assets/icons';
import useAuthStore from '../store.js'
import axios from '../utils/axios.js'
import AsyncStorage from '@react-native-async-storage/async-storage';

const Login = () => {
  const router = useRouter();
  const login=useAuthStore((state)=>state.login);
  const [Email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const onSubmit = async () => {
    console.log("login button clicked")
    // Log the values to check if they're getting updated correctly
    console.log("Email:", Email);
   

    if (!Email || !password) {
      Alert.alert('Login', "Please fill all the fields!");
      return;
    }

    // Start loading (show the loading spinner)
    setLoading(true);

    try {
      console.log("making Api call");
      // Make the API call to login
      const response = await axios.post('/api/Login/login', {
        Email: Email,
        password: password,
      });
      console.log("API response",response.data);
      // If the response is successful, navigate to the dashboard
      if (response.data.success) {
        // Store the token in AsyncStorage (if provided)
        if (response.data.token) {
          console.log("storing token");
          await AsyncStorage.setItem('token', response.data.token);
        }
        login({
          id: response.data.user.id,
          Uid:response.data.user.uqid,
          role: response.data.user.role,
          email:response.data.user.email,
        })
        console.log("NAvigating to homepage")
        // Navigate to the home page
        router.push('/homePage');
      } else {
        // Show an error message if the login failed
        Alert.alert('Login Failed', response.data.message || "Invalid email or password");
      }
    } catch (error) {
      // Handle network errors or other issues
      console.error('Login Error:', error);

      // Show a user-friendly error messagew
      if (error.response) {
        // Backend returned an error response
        Alert.alert('Error', error.request || 'Something went wrong. Please try again.');
      } else if (error.request) {
        // No response received from the backend
        Alert.alert('Error', 'No response from the server. Please check your connection.');
      } else {
        // Other errors (e.g., axios configuration issues)
        Alert.alert('Error', 'Something went wrong. Please try again.');
      }
    } finally {
      // Stop loading regardless of success or failure
      setLoading(false);
    }
  };

  return (
    <ScreenWrapper bg="white">
      <StatusBar style="dark" />
      <View style={styles.container}>
        {/* Back Button */}
        <BackButton router={router} />

        {/* Welcome Back Text */}
        <View>
          <Text style={styles.welcomeText}>Welcome Back!</Text>
        </View>

        {/* Login Form */}
        <View style={styles.form}>
          <Text style={{ fontSize: hp(1.5), color: theme.colors.text }}>
            Please Log In to continue
          </Text>
          <Input
            icon={<Icon name="mail" size={26} strokeWidth={1.6} />}
            placeholder="Enter your email"
            onChangeText={setEmail}
          />
          <Input
            icon={<Icon name="lock" size={26} strokeWidth={1.6} />}
            placeholder="Enter your password"
            secureTextEntry
            onChangeText={setPassword}
          />
          <Pressable onPress={() => router.push('/forgotPassword')}>
            <Text style={styles.forgotPassword}>
              Forgot Password?
            </Text>
          </Pressable>

          {/* Button */}
          <Button titles={'Login'} loading={loading} onPress={onSubmit} />

          {/* Footer with Sign Up link */}
          <View style={styles.footer}>
            <Text style={styles.footerText}>
              Don't have an account?
            </Text>
            <Pressable onPress={() => router.push('/welcome2')}>
              <Text style={[styles.footerText, { color: theme.colors.primaryDark, fontWeight: theme.fonts.semibold }]}>
                Sign Up
              </Text>
            </Pressable>
          </View>
        </View>
      </View>
    </ScreenWrapper>
  );
};

export default Login;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 30,
    paddingHorizontal: wp(2),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  welcomeText: {
    fontSize: hp(4),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  form: {
    gap: 20,
  },
  forgotPassword: {
    textAlign: 'right',
    fontWeight: theme.fonts.semibold,
    color: theme.colors.text,
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    gap: 5,
  },
  footerText: {
    textAlign: 'center',
    color: theme.colors.text,
    fontSize: hp(1.6),
  },
});
