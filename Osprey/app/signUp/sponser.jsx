import React, { useState } from "react";
import { Alert, StyleSheet, Text, View, ScrollView, KeyboardAvoidingView, Platform, Pressable } from "react-native";
import ScreenWrapper from "../../components/ScreenWrapper";
import { theme } from "../../constants/theme";
import { StatusBar } from "expo-status-bar";
import { hp, wp } from "../../helpers/common";
import Input from "../../components/input";
import { useRouter } from "expo-router"; 
import Button from "../../components/Button";
// Import icons from react-native-vector-icons
import { FontAwesome, Ionicons } from 'react-native-vector-icons';  
import BackButton from "../../components/BackButton";
import Icon from "../../assets/icons";
import axios from '../../utils/axios.js';

const SponsorSignUp = () => {
  const router = useRouter();
  
  // Form data state for sponsor
  const [formData, setFormData] = useState({
    companyName: "",
    contactName: "",
    email: "",
    phone: "",
    website: "",
    address: "",
    sponsorshipInterest: "",
    password:"",
  });

  const [loading, setLoading] = useState(false);
  const [currentSlide, setCurrentSlide] = useState(0); // Tracking the current slide

  const validateEmail = (email) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  };

  const onChangeText = (field, value) => {
    setFormData(prevState => ({
      ...prevState,
      [field]: value
    }));
  };

  const onSubmit = async () => {
    if (!validateEmail(formData.email)) {
      Alert.alert("Invalid Email", "Please enter a valid email address.");
      return;
    }
    if (!formData.companyName || !formData.contactName || !formData.email || !formData.phone || !formData.website || !formData.address) {
      Alert.alert("Sign Up", "Please fill all the fields!");
      return;
    }

    setLoading(true);
    try {
      console.log("Making API call");

      //Make the API call to login
     // Make the API call to login
          const response = await axios.post('/api/BusinessOwner', {
            Username : formData.contactName,
            CompanyName : formData.companyName,
            Address: formData.address,
            website : formData.website,
            ContactNumber: formData.phone,
            country: formData.country,   
            email: formData.email,
            password: formData.password,
            sponsorshipInterest : formData.sponsorshipInterest
          });
          console.log("API response");
          // If the response is successful, navigate to the dashboard
          if (response.data.success) {
            // Store the token in AsyncStorage (if provided)
           if (response.data.token){
            console.log("Storing token")
            await AsyncStorage.setItem('Token',response.data.token);
           }
          
            console.log("NAvigating to homepage")
            // Navigate to the home page
            router.push('/login');
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
  const handleNext = () => {
    setCurrentSlide((prev) => prev + 1);
  };

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
            <Text style={styles.welcomeText}>Get Started as Sponsor</Text>
          </View>
          
          {/* Slide 1 - Sponsor's Basic Information */}
          {currentSlide === 0 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Company's Basic Information</Text>

              <Input
                icon={<FontAwesome name="building" size={26} color={theme.colors.text} />}
                placeholder="Enter Company Name"
                value={formData.companyName}
                onChangeText={(value) => onChangeText("companyName", value)}
              />
              <Input
                icon={<FontAwesome name="user" size={26} color={theme.colors.text} />}
                placeholder="Enter Contact Person's Name"
                value={formData.contactName}
                onChangeText={(value) => onChangeText("contactName", value)}
              />
              <Input
                icon={<FontAwesome name="phone" size={26} color={theme.colors.text} />}
                placeholder="Enter Phone Number"
                value={formData.phone}
                onChangeText={(value) => onChangeText("phone", value)}
              />
              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {/* Slide 2 - Credentials and Business Info */}
          {currentSlide === 1 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Sponsorship Details</Text>

              <Input
                icon={<FontAwesome name="envelope" size={26} color={theme.colors.text} />}
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
              <Input
                icon={<Icon name="location" size={26} strokeWidth={1.6} />}
                placeholder="Enter Website URL"
                value={formData.website}
                onChangeText={(value) => onChangeText("website", value)}
              />
              <Input
                icon={<FontAwesome name="briefcase" size={26} color={theme.colors.text} />}
                placeholder="Enter Business Type (e.g., Corporate)"
                value={formData.address}
                onChangeText={(value) => onChangeText("address", value)}
              />
              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {/* Slide 3 - Sponsorship Interest and Final Submit */}
          {currentSlide === 2 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Sponsorship Interests</Text>

              <Input
                icon={<FontAwesome name="heart" size={26} color={theme.colors.text} />}
                placeholder="Enter Sponsorship Interests (e.g., Events, Teams)"
                value={formData.sponsorshipInterest}
                onChangeText={(value) => onChangeText("sponsorshipInterest", value)}
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

export default SponsorSignUp;

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
