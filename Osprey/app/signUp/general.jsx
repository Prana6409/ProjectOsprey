import React, { useState } from "react";
import { Alert, StyleSheet, Text, View, ScrollView, KeyboardAvoidingView, Platform, Pressable } from "react-native";
import ScreenWrapper from "../../components/ScreenWrapper";
import { theme } from "../../constants/theme";
import { StatusBar } from "expo-status-bar";
import { hp, wp } from "../../helpers/common";
import Input from "../../components/input";
import { useRouter } from "expo-router"; 
import Button from "../../components/Button";
import { FontAwesome, Ionicons } from 'react-native-vector-icons';  
import BackButton from "../../components/BackButton";
import Icon from "../../assets/icons";
import DatePickerField from "../../components/date"; // Ensure this uses react-native-modal-datetime-picker
import axios from '../../utils/axios.js';
import AsyncStorage from '@react-native-async-storage/async-storage';

const General = () => {
  const router = useRouter();
  
  // Form data state
  const [formData, setFormData] = useState({
    name: "",
    address: "",
    phone: "",
    email: "",
    password: "",
    country: "",
    dateofbirth: new Date().toISOString(), // Initialize with a valid date
  });

  const [loading, setLoading] = useState(false);
  const [currentSlide, setCurrentSlide] = useState(0); // Tracking the current slide

  // Validate email format
  const validateEmail = (email) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  };

  // Handle form field changes
  const onChangeText = (field, value) => {
    let formattedValue = value;

    // If the value is a Date object, format it as YYYY-MM-DD
    if (value instanceof Date) {
      const year = value.getFullYear();
      const month = String(value.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
      const day = String(value.getDate()).padStart(2, '0');
      formattedValue = `${year}-${month}-${day}`;
    }

    setFormData(prevState => ({
      ...prevState,
      [field]: formattedValue,
    }));
  };

  // Handle form submission
  const onSubmit = async () => {
    console.log("Login button clicked");
     
    console.log("Email:", formData.email);
    // Log the form data being sent
    console.log("Form Data:", formData);

    // Validate email
    if (!validateEmail(formData.email)) {
      Alert.alert("Invalid Email", "Please enter a valid email address.");
      return;
    }

    // Validate required fields
    if (!formData.name || !formData.address || !formData.phone || !formData.password) {
      Alert.alert("Sign Up", "Please fill all the fields!");
      return;
    }

    setLoading(true);
    try {
      console.log("Making API call");

      // Log the payload being sent to the server
      const payload = {
        Name: formData.name,
        address: formData.address,
        dateofbirth: formData.dateofbirth,
        number: formData.phone,
        country: formData.country,
        email: formData.email,
        password: formData.password,
      };
      console.log("Payload:", payload);

      // Make the API call to sign up
      const response = await axios.post('/api/Users', payload);
      console.log("API response:", response.data);

      // If the response is successful, navigate to the dashboard
      if (response.data.success) {
        // Store the token in AsyncStorage (if provided)
        if (response.data.token) {
          console.log("Storing token");
          await AsyncStorage.setItem('Token', response.data.token);
        }

        console.log("Navigating to homepage");
        // Navigate to the home page
        router.push('/login');
      } else {
        // Show an error message if the sign-up failed
        Alert.alert('Sign Up Failed', response.data.message || "Invalid email or password");
      }
    } catch (error) {
      // Handle network errors or other issues
      console.error('Sign Up Error:', error);

      // Log the server's response (if available)
      if (error.response) {
        console.error("Server Response:", error.response.data);
      }

      // Show a user-friendly error message
      if (error.response) {
        Alert.alert('Error', error.response.data.message || 'Something went wrong. Please try again.');
      } else if (error.request) {
        Alert.alert('Error', 'No response from the server. Please check your connection.');
      } else {
        Alert.alert('Error', 'Something went wrong. Please try again.');
      }
    } finally {
      // Stop loading regardless of success or failure
      setLoading(false);
    }
  };

  // Handle next slide
  const handleNext = () => {
    setCurrentSlide((prev) => prev + 1);
  };

  // Handle previous slide
  const handleBack = () => {
    setCurrentSlide((prev) => prev - 1);
  };

  return (
    <ScreenWrapper>
      <StatusBar style="dark" />
      <KeyboardAvoidingView
        behavior={Platform.OS === "ios" ? "padding" : "height"}
        style={{ flex: 1 }}
      >
        <ScrollView style={styles.container}>
          <BackButton router={router} />
          <View style={styles.form}>
            <Text style={styles.welcomeText}>Get Started as General User</Text>
          </View>
          
          {/* Slide 1 - Basic Information */}
          {currentSlide === 0 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Basic Information</Text>

              <Input
                icon={<Icon name="user" size={26} strokeWidth={1.6} />}
                placeholder="Enter a Username"
                value={formData.name}
                onChangeText={(value) => onChangeText("name", value)}
              />
              <Input
                icon={<Icon name="location" size={26} strokeWidth={1.6} />}
                placeholder="Enter your Primary Address"
                value={formData.address}
                onChangeText={(value) => onChangeText("address", value)}
              />
              <Input
                icon={<Ionicons name="airplane-outline" size={26} color={theme.colors.text} />}
                placeholder="Country of Residence"
                value={formData.country}
                onChangeText={(value) => onChangeText("country", value)}
              />
              <Input
                icon={<Icon name="call" size={26} strokeWidth={1.6} />}
                placeholder="Enter your Phone Number"
                value={formData.phone}
                onChangeText={(value) => onChangeText("phone", value)}
              />
              <DatePickerField
                value={formData.dateofbirth}
                onChange={(date) => {
                  console.log("Selected Date:", date); // Log the selected date
                  onChangeText("dateofbirth", date);
                }}
              />
              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {/* Slide 2 - Credentials */}
          {currentSlide === 1 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Credentials</Text>

              <Input
                icon={<Icon name="mail" size={26} strokeWidth={1.6} />}
                placeholder="Enter your Email"
                value={formData.email}
                onChangeText={(value) => onChangeText("email", value)}
              />
              <Input
                icon={<Icon name="lock" size={26} strokeWidth={1.6} />}
                placeholder="Enter your Password"
                secureTextEntry
                value={formData.password}
                onChangeText={(value) => onChangeText("password", value)}
              />
              <Pressable onPress={handleBack}>
                <Text style={styles.backText}>Back</Text>
              </Pressable>
              <Button titles="Sign Up" onPress={onSubmit} loading={loading} />
            </View>
          )}
        </ScrollView>
      </KeyboardAvoidingView>
    </ScreenWrapper>
  );
};

export default General;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 30,
    paddingHorizontal: wp(2),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  slide: {
    gap: 15,
  },
  backText: {
    fontSize: hp(2),
    textAlign: "center",
    color: theme.colors.primary,
    marginBottom: hp(2),
  },
  formText: {
    fontSize: hp(2),
    color: theme.colors.gray,
    gap: 10,
  },
  welcomeText: {
    fontSize: hp(3),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  form: {
    marginVertical: hp(6),
  },
});