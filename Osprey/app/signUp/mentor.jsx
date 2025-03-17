import React, { useState } from "react";
import { Alert, StyleSheet, Text, View, ScrollView, KeyboardAvoidingView, Platform, Pressable } from "react-native";
import ScreenWrapper from "../../components/ScreenWrapper";
import { theme } from "../../constants/theme";
import { StatusBar } from "expo-status-bar";
import { hp, wp } from "../../helpers/common";
import Input from "../../components/input";
import { useRouter } from "expo-router"; 
import Button from "../../components/Button";
import DatePickerField from "../../components/date";

import { FontAwesome, Ionicons, MaterialIcons, AntDesign, Feather } from 'react-native-vector-icons';
import BackButton from "../../components/BackButton";
import Icon from "../../assets/icons";
import axios from '../../utils/axios.js';

import AsyncStorage from '@react-native-async-storage/async-storage';

const Mentors = () => {
  const router = useRouter();
  
  // State variables for form fields
  const [formData, setFormData] = useState({
    name: "",
    address: "",
    phone: "",
    email: "",
    password: "",
    stageName: "",
    Talent: "",
    Sport: "",
    team: "",
    country:"",
    dateofbirth:"",
  });

  const [loading, setLoading] = useState(false);
  const [category, setCategory] = useState("Athletes"); // Default category "Athletes"
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
    // Validate email
    if (!validateEmail(formData.email)) {
      Alert.alert("Invalid Email", "Please enter a valid email address.");
      return;
    }
  
    // Validate password
    if (!validatePassword(formData.password)) {
      Alert.alert("Invalid Password", "Password must be at least 8 characters long and contain at least one letter and one number.");
      return;
    }
  
    // Validate required fields
    if (!formData.name || !formData.address || !formData.phone || !formData.password || !formData.country || !formData.dateofbirth) {
      Alert.alert("Sign Up", "Please fill all the fields!");
      return;
    }
  
    // Validate category-specific fields
    if (category === "Entertainment" && (!formData.stageName || !formData.genre)) {
      Alert.alert("Error", "Please fill all Entertainment-specific fields.");
      return;
    }
  
    if (category === "Athletes" && (!formData.Sport || !formData.team)) {
      Alert.alert("Error", "Please fill all Athlete-specific fields.");
      return;
    }
  
    setLoading(true);
  
    // Prepare the common data object
    const data = {
      Username: formData.name,
      Address: formData.address,
      ContactNumber: formData.phone,
      email: formData.email,
      password: formData.password,
      Country: formData.country,
      DateOfBirth: formData.dateofbirth,
    };
  
    // Add category-specific fields
    if (category === "Entertainment") {
      data.stageName = formData.stageName;
      data.Talent = formData.genre;
    } else if (category === "Athletes") {
      data.Sport = formData.Sport;
      data.team = formData.team;
    }
  
    try {
      let response;
      if (category === "Entertainment") {
        response = await axios.post("/api/Entertainer", data);
      } else if (category === "Athletes") {
        response = await axios.post("/api/Sportsman", data);
      }
  
      if (response.data.success) {
        if (response.data.token) {
          await AsyncStorage.setItem("Token", response.data.token);
        }
        Alert.alert("Success", `${category} account created successfully!`);
        router.push("/login");
      } else {
        Alert.alert("Login Failed", response.data.message || "Invalid email or password");
      }
    } catch (error) {
      Alert.alert("Error", error.message || "There was an issue submitting your details. Please try again.");
    } finally {
      setLoading(false);
    }
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
          <View>
            <Text style={styles.welcomeText}>Get Started as Mentor</Text>
          </View>

          {/* Render slides based on currentSlide */}
          {currentSlide === 0 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Basic Information</Text>

              <Input
                icon={<Icon name="user" size={26} strokeWidth={1.6} />}
                placeholder="Enter your Full Name"
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
                icon={<FontAwesome name="phone" size={26} color={theme.colors.text} />}
                placeholder="Enter your Phone Number"
                value={formData.phone}
                onChangeText={(value) => onChangeText("phone", value)}
              />
               <DatePickerField router={router}
               value={formData.Birthdate}  // Pass Birthdate value here
               onChange={(date) => onChangeText("Birthdate", date)}  // Update Birthdate in state
             />

              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {currentSlide === 1 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Your Credentials</Text>

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
              <Pressable onPress={handleBack}>
                <Text style={styles.backText}>Back</Text>
              </Pressable>
              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {currentSlide === 2 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Select Your Category</Text>

              <Pressable
                onPress={() => setCategory("Entertainment")}
                style={[styles.categoryButton, category === "Entertainment" && styles.selectedCategory]}
              >
                <Text style={category === "Entertainment" ? styles.selectedText : styles.categoryText}>
                  Entertainment
                </Text>
              </Pressable>
              <Pressable
                onPress={() => setCategory("Athletes")}
                style={[styles.categoryButton, category === "Athletes" && styles.selectedCategory]}
              >
                <Text style={category === "Athletes" ? styles.selectedText : styles.categoryText}>
                  Athlete
                </Text>
              </Pressable>

              <Pressable onPress={handleBack}>
                <Text style={styles.backText}>Back</Text>
              </Pressable>
              <Button titles="Next" onPress={handleNext} />
            </View>
          )}

          {currentSlide === 3 && (
            <View style={styles.slide}>
              <Text style={styles.formText}>Enter Additional Information</Text>

              {category === "Entertainment" && (
                <>
                
                  <Input
                    icon={<Ionicons name="musical-note" size={26} color={theme.colors.text} />}
                    placeholder="Enter Your Stage Name"
                    value={formData.stageName}
                    onChangeText={(value) => onChangeText("stageName", value)}
                  />
                  <Input
                    icon={<FontAwesome name="star" size={26} color={theme.colors.text} />}
                    placeholder="Enter Your Talent"
                    value={formData.genre}
                    onChangeText={(value) => onChangeText("Talent", value)}
                  />
                </>
              )}

              {category === "Athletes" && (
                <>
                  <Input
                    icon={<MaterialIcons name="Sports-soccer" size={26} color={theme.colors.text} />}
                    placeholder="Enter Your Sport"
                    value={formData.Sport}
                    onChangeText={(value) => onChangeText("Sport", value)}
                  />
                  <Input
                    icon={<FontAwesome name="medal" size={26} color={theme.colors.text} />}
                    placeholder="Enter Your Team (if applicable)"
                    value={formData.team}
                    onChangeText={(value) => onChangeText("team", value)}
                  />
                </>
              )}

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

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 30,
    paddingHorizontal: wp(2),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  welcomeText: {
    fontSize: hp(3),
    fontWeight: "bold",
    marginBottom: hp(2),
    textAlign: "center",
  },
  
  formText: {
    fontSize: hp(2),
    textAlign: "center",
    color: theme.colors.gray,
  },
  slide: {
    flex: 1,

    paddingVertical: hp(4),
    gap:20
  },
  backText: {
    fontSize: hp(2),
    textAlign: "center",
    color: theme.colors.primary,
    marginBottom: hp(2),
  },
  categoryButton: {
    paddingVertical: hp(1),
    paddingHorizontal: wp(4),
    borderRadius: 30,
    borderWidth: 1,
    borderColor: theme.colors.lightGray,
    backgroundColor: theme.colors.white,
    marginBottom: hp(2),
  },
  selectedCategory: {
    backgroundColor: 'lightgoldenrodyellow', // light gold background color
    borderColor: 'gold',
  },
  selectedText: {
    color: theme.colors.primary,
    fontWeight: 'bold',
  },
  categoryText: {
    color: theme.colors.text,
  },
});

export default Mentors;
