import React, { useEffect, useState } from 'react';
import { View, Text, Image, StyleSheet, ScrollView, Alert } from 'react-native';
import ScreenWrapper from '../../components/ScreenWrapper';
import { useRouter } from 'expo-router';
import { wp } from '../../helpers/common';
import { theme } from '../../constants/theme';
import BackButton from '../../components/BackButton';
import Button from '../../components/Button';
import useAuthStore from '../../store'; // Import Zustand store
import axios from '../../utils/axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const ProfileScreen = () => {
  const router = useRouter();
  const { user, logout } = useAuthStore((state) => ({
    user: state.user, // Get user data from Zustand
    logout: state.logout, // Get logout action from Zustand
  }));
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  // Fetch profile data based on user role and ID
  useEffect(() => {
    const fetchProfile = async () => {
      if (!user) {
        router.push('/login'); // Redirect to login if no user is authenticated
        return;
      }

      try {
        const response = await axios.get(`/api/profile/${user.role.toLowerCase()}/${user.uqid}`);
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

  const handleLogout = async () => {
    try {
      await AsyncStorage.removeItem('token'); // Clear token from AsyncStorage
      logout(); // Clear user data from Zustand
      router.push('/login'); // Redirect to login
    } catch (error) {
      console.error('Logout Error:', error);
      Alert.alert('Error', 'Failed to logout. Please try again.');
    }
  };

  const handleEditProfile = () => {
    router.push('/edit-profile');
  };

  if (loading) {
    return (
      <ScreenWrapper>
        <Text>Loading...</Text>
      </ScreenWrapper>
    );
  }

  if (!profile) {
    return (
      <ScreenWrapper>
        <Text>Failed to load profile data.</Text>
      </ScreenWrapper>
    );
  }

  return (
    <ScreenWrapper>
      {/* Back button */}
      <BackButton router={router} />

      <ScrollView contentContainerStyle={styles.container}>
        {/* Profile Picture */}
        <View style={styles.profilePictureContainer}>
          <Image
            source={profile.profilePicture ? { uri: profile.profilePicture } : require('../../assets/images/profile.jpeg')}
            style={styles.profilePicture}
          />
        </View>

        {/* User Name */}
        <Text style={styles.name}>{profile.name}</Text>

        {/* Bio */}
        <Text style={styles.bio}>{profile.bio}</Text>

        {/* Profile Details */}
        <View style={styles.detailsContainer}>
          <Text style={styles.detailTitle}>Email:</Text>
          <Text style={styles.detailText}>{profile.email}</Text>

          <Text style={styles.detailTitle}>Phone:</Text>
          <Text style={styles.detailText}>{profile.phone}</Text>

          <Text style={styles.detailTitle}>Location:</Text>
          <Text style={styles.detailText}>{profile.location}</Text>
        </View>

        {/* Edit Button */}
        <Button title="Edit Profile" onPress={handleEditProfile} />

        {/* Logout Button */}
        <Button title="Logout" onPress={handleLogout} buttonStyle={styles.logoutButton} />
      </ScrollView>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingHorizontal: wp(2),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  profilePictureContainer: {
    marginTop: wp(7),
    alignItems: 'center',
    marginBottom: 20,
  },
  profilePicture: {
    width: 120,
    height: 120,
    borderRadius: 60,
    borderWidth: 2,
    borderColor: '#ccc',
  },
  name: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 3,
  },
  bio: {
    fontSize: 16,
    textAlign: 'center',
    marginBottom: 20,
    color: '#666',
  },
  detailsContainer: {
    marginBottom: 30,
    alignItems: 'center',
  },
  detailTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 5,
  },
  detailText: {
    fontSize: 14,
    color: '#333',
    marginBottom: 3,
  },
  logoutButton: {
    backgroundColor: 'gold',
    marginTop: 1,
  },
});

export default ProfileScreen;