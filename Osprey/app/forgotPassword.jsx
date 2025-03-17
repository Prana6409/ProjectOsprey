
import React, { useState } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, Alert } from 'react-native';
import { wp, hp } from '../helpers/common'; 
import { theme } from '../constants/theme'; 
import Button from '../components/Button';
import { useRouter } from 'expo-router';
import ScreenWrapper from '../components/ScreenWrapper';

const ForgotPasswordScreen = () => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  const handleSubmit = async () => {
    if (!email) {
      Alert.alert('Error', 'Please enter your email address.');
      return;
    }

    setLoading(true);

    // Simulating an API call for resetting password
    setTimeout(() => {
      setLoading(false);
      Alert.alert('Success', 'Password reset link has been sent to your email.');
      router.push('/login'); // Navigate to login page after successful request
    }, 2000); // Simulate API call delay
  };

  return (
    <ScreenWrapper>
    <View style={styles.container}>
      <Text style={styles.title}>Forgot Password</Text>
      <Text style={styles.subtitle}>Enter your email to reset your password</Text>

      <TextInput
        style={styles.input}
        placeholder="Enter your email"
        value={email}
        onChangeText={(text) => setEmail(text)}
        keyboardType="email-address"
        autoCapitalize="none"
      />

      {/* Reset Password Button */}
      <Button titles="Reset Password" loading={loading} onPress={handleSubmit} />

      <TouchableOpacity onPress={() => router.push('/login')} style={styles.backToLogin}>
        <Text style={styles.backToLoginText}>Back to Login</Text>
      </TouchableOpacity>
    </View>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: wp(5),
    paddingVertical: hp(3),
    backgroundColor: theme.colors.background,
  },
  title: {
    fontSize: hp(3),
    fontWeight: 'bold',
    marginBottom: hp(2),
    color: theme.colors.text,
  },
  subtitle: {
    fontSize: hp(1.8),
    color: theme.colors.text,
    marginBottom: hp(4),
    textAlign: 'center',
  },
  input: {
    width: '100%',
    padding: hp(1.5),
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    marginBottom: hp(3),
    fontSize: hp(1.8),
    color: theme.colors.text,
  },
  backToLogin: {
    marginTop: hp(2),
  },
  backToLoginText: {
    fontSize: hp(1.8),
    color: theme.colors.primaryDark,
    fontWeight: 'bold',
  },
  
});

export default ForgotPasswordScreen;
