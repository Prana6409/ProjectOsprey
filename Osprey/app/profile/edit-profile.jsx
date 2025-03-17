import React, { useEffect, useState } from 'react';
import { View, Text, TextInput, StyleSheet, Alert, ScrollView } from 'react-native';
import { useRouter } from 'expo-router';
import { wp } from '../../helpers/common';
import { theme } from '../../constants/theme';
import Button from '../../components/Button';
import BackButton from '../../components/BackButton';
import useAuthStore from '../../store'; // Import Zustand store
import axios from '../../utils/axios';

const EditProfileScreen = () => {
  const router = useRouter();
  const { user } = useAuthStore((state) => ({ user: state.user })); // Get user data from Zustand
  const [profile, setProfile] = useState({
    name: '',
    bio: '',
    email: '',
    phone: '',
    location: '',
  });
  const [loading, setLoading] = useState(true);

  // Fetch profile data based on user role and ID
  useEffect(() => {
    const fetchProfile = async () => {
      if (!user) {
        router.push('/login'); // Redirect to login if no user is authenticated
        return;
      }

      try {
        const response = await axios.get(`/api/${user.role}/${user.id}`);
        if (response.data.success) {
          setProfile(response.data.profile); // Set profile data
        } else {
          Alert.alert('Error', response.data.message || 'Failed to fetch profile data.');
        }
      } catch (error) {
        console.error('Fetch Profile Error:', error);
        Alert.alert('Error', 'Something went wrong. Please try again.');
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [user]);

  const handleInputChange = (field, value) => {
    setProfile((prevState) => ({
      ...prevState,
      [field]: value,
    }));
  };

  const handleSaveProfile = async () => {
    try {
      const response = await axios.put(`/api/profile/${user.role.toLowerCase()}/${user.id}`, profile);
      if (response.data.success) {
        Alert.alert('Profile Updated', 'Your profile has been updated successfully.');
        router.push('/profile'); // Navigate back to the profile screen after saving
      } else {
        Alert.alert('Error', response.data.message || 'Failed to update profile.');
      }
    } catch (error) {
      console.error('Update Profile Error:', error);
      Alert.alert('Error', 'Something went wrong. Please try again.');
    }
  };

  if (loading) {
    return (
      <View style={styles.container}>
        <Text>Loading...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      {/* Back Button */}
      <BackButton router={router} />

      <ScrollView contentContainerStyle={styles.formContainer}>
        <Text style={styles.heading}>Edit Profile</Text>

        {/* Name Field */}
        <Text style={styles.label}>Name</Text>
        <TextInput
          style={styles.input}
          value={profile.name}
          onChangeText={(text) => handleInputChange('name', text)}
          placeholder="Enter your name"
        />

        {/* Bio Field */}
        <Text style={styles.label}>Bio</Text>
        <TextInput
          style={styles.input}
          value={profile.bio}
          onChangeText={(text) => handleInputChange('bio', text)}
          placeholder="Tell us about yourself"
          multiline
        />

        {/* Email Field */}
        <Text style={styles.label}>Email</Text>
        <TextInput
          style={styles.input}
          value={profile.email}
          onChangeText={(text) => handleInputChange('email', text)}
          placeholder="Enter your email"
        />

        {/* Phone Field */}
        <Text style={styles.label}>Phone</Text>
        <TextInput
          style={styles.input}
          value={profile.phone}
          onChangeText={(text) => handleInputChange('phone', text)}
          placeholder="Enter your phone number"
        />

        {/* Location Field */}
        <Text style={styles.label}>Location</Text>
        <TextInput
          style={styles.input}
          value={profile.location}
          onChangeText={(text) => handleInputChange('location', text)}
          placeholder="Enter your location"
        />

        {/* Save Button */}
        <Button title="Save Changes" onPress={handleSaveProfile} />
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: wp(4),
  },
  formContainer: {
    marginTop: wp(6),
    paddingHorizontal: wp(4),
  },
  heading: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: wp(5),
  },
  label: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: wp(2),
    color: theme.colors.text,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    padding: wp(3),
    borderRadius: 8,
    fontSize: 16,
    marginBottom: wp(4),
    color: '#333',
  },
});

export default EditProfileScreen;