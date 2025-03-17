import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  Image,
  StyleSheet,
  Alert,
  ScrollView,
} from 'react-native';
import { useRouter } from 'expo-router'; // Use Expo Router's useRouter
import * as ImagePicker from 'expo-image-picker'; // For image picking
import ScreenWrapper from '../../components/ScreenWrapper';
import Header from '../../components/Header';

export default function NewEventScreen() {
  const router = useRouter(); // Initialize router

  // State for event details
  const [eventTitle, setEventTitle] = useState('');
  const [eventDate, setEventDate] = useState('');
  const [eventLocation, setEventLocation] = useState('');
  const [eventDescription, setEventDescription] = useState('');
  const [eventImage, setEventImage] = useState(null);
  const [loading, setLoading] = useState(false); // Loading state for backend submission

  // Function to handle image picking
  const pickImage = async () => {
    // Request permission to access the media library
    const { status } = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (status !== 'granted') {
      Alert.alert('Permission Denied', 'Sorry, we need camera roll permissions to upload an image.');
      return;
    }

    // Launch the image picker
    let result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsEditing: true,
      aspect: [4, 3], // Adjust aspect ratio as needed
      quality: 0.5, // Reduce image size
    });

    if (!result.canceled) {
      setEventImage(result.assets[0].uri); // Set the selected image URI
    }
  };

  // Function to handle event submission
  const handleSubmitEvent = async () => {
    if (!eventTitle.trim() || !eventDate.trim() || !eventLocation.trim()) {
      Alert.alert('Error', 'Please fill in all required fields.');
      return;
    }

    setLoading(true); // Start loading

    // Backend Integration Example:
    /*
    const formData = new FormData();
    formData.append('title', eventTitle);
    formData.append('date', eventDate);
    formData.append('location', eventLocation);
    formData.append('description', eventDescription);
    if (eventImage) {
      formData.append('image', {
        uri: eventImage,
        name: 'event.jpg',
        type: 'image/jpeg',
      });
    }

    try {
      const response = await fetch('https://your-backend-api.com/events', {
        method: 'POST',
        headers: {
          'Content-Type': 'multipart/form-data',
          'Authorization': `Bearer ${userToken}`, // Add authentication token if needed
        },
        body: formData,
      });

      const data = await response.json();

      if (response.ok) {
        Alert.alert('Success', 'Your event has been created successfully.');
        router.back(); // Navigate back to the previous screen
      } else {
        throw new Error(data.message || 'Failed to create event');
      }
    } catch (error) {
      console.error('Error creating event:', error);
      Alert.alert('Error', 'An error occurred while creating your event. Please try again.');
    } finally {
      setLoading(false); // Stop loading
    }
    */

    // Simulate backend call for now
    setTimeout(() => {
      Alert.alert('Success', 'Your event has been created successfully.');
      router.back(); // Navigate back to the previous screen
      setLoading(false); // Stop loading
    }, 1000);
  };

  return (
    <ScreenWrapper>
      <Header title="New Event" />
    <ScrollView contentContainerStyle={styles.container}>
      {/* Event Title Input */}
      <TextInput
        style={styles.input}
        value={eventTitle}
        onChangeText={setEventTitle}
        placeholder="Event Title"
      />

      {/* Event Date Input */}
      <TextInput
        style={styles.input}
        value={eventDate}
        onChangeText={setEventDate}
        placeholder="Event Date (e.g., 2023-10-15)"
      />

      {/* Event Location Input */}
      <TextInput
        style={styles.input}
        value={eventLocation}
        onChangeText={setEventLocation}
        placeholder="Event Location"
      />

      {/* Event Description Input */}
      <TextInput
        style={[styles.input, styles.descriptionInput]}
        value={eventDescription}
        onChangeText={setEventDescription}
        placeholder="Event Description"
        multiline
      />

      {/* Image Picker */}
      <TouchableOpacity style={styles.imagePickerButton} onPress={pickImage}>
        <Text style={styles.imagePickerText}>Upload Event Image</Text>
      </TouchableOpacity>

      {/* Display Selected Image */}
      {eventImage && (
        <Image source={{ uri: eventImage }} style={styles.selectedImage} />
      )}

      {/* Submit Button */}
      <TouchableOpacity
        style={styles.submitButton}
        onPress={handleSubmitEvent}
        disabled={loading} // Disable button while loading
      >
        {loading ? (
          <ActivityIndicator color="#fff" /> // Show loading indicator
        ) : (
          <Text style={styles.submitButtonText}>Create Event</Text>
        )}
      </TouchableOpacity>
    </ScrollView>
    </ScreenWrapper>
  );
}

const styles = StyleSheet.create({
  container: {
    flexGrow: 1,
    padding: 16,
    backgroundColor: '#f0f0f0',
  },
  input: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 8,
    fontSize: 16,
    marginBottom: 16,
  },
  descriptionInput: {
    height: 100,
    textAlignVertical: 'top', // For multiline input
  },
  imagePickerButton: {
    backgroundColor: 'orange',
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
    marginBottom: 16,
  },
  imagePickerText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  selectedImage: {
    width: '100%',
    height: 200,
    borderRadius: 8,
    marginBottom: 16,
    resizeMode: 'cover',
  },
  submitButton: {
    backgroundColor: 'orange',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
  },
  submitButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});