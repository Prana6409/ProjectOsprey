import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Image, StyleSheet, Alert, ScrollView } from 'react-native';
import { useRouter } from 'expo-router'; // Use Expo Router's useRouter
import * as ImagePicker from 'expo-image-picker'; // For image picking
import ScreenWrapper from '../../components/ScreenWrapper';
import Header from '../../components/Header';

export default function NewPostScreen() {
  const router = useRouter(); // Initialize router

  // State for post content and image
  const [postContent, setPostContent] = useState('');
  const [postImage, setPostImage] = useState(null);

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
      setPostImage(result.assets[0].uri); // Set the selected image URI
    }
  };

  // Function to handle post submission
  const handleSubmitPost = () => {
    if (!postContent.trim()) {
      Alert.alert('Error', 'Please enter some content for your post.');
      return;
    }

    // Backend Integration Example:
    /*
    const formData = new FormData();
    formData.append('content', postContent);
    if (postImage) {
      formData.append('image', {
        uri: postImage,
        name: 'post.jpg',
        type: 'image/jpeg',
      });
    }

    fetch('https://your-backend-api.com/posts', {
      method: 'POST',
      headers: {
        'Content-Type': 'multipart/form-data',
        'Authorization': `Bearer ${userToken}`, // Add authentication token if needed
      },
      body: formData,
    })
      .then((response) => response.json())
      .then((data) => {
        Alert.alert('Success', 'Your post has been created successfully.');
        router.back(); // Navigate back to the previous screen
      })
      .catch((error) => {
        console.error('Error creating post:', error);
        Alert.alert('Error', 'An error occurred while creating your post. Please try again.');
      });
    */

    // Simulate backend call for now
    Alert.alert('Success', 'Your post has been created successfully.');
    router.back(); // Navigate back to the previous screen
  };

  return (
    <ScreenWrapper>
    <Header title="New Post" />
    <ScrollView contentContainerStyle={styles.container}>
      {/* Post Content Input */}
      <TextInput
        style={styles.input}
        value={postContent}
        onChangeText={setPostContent}
        placeholder="What's on your mind?"
        multiline
      />

      {/* Image Picker */}
      <TouchableOpacity style={styles.imagePickerButton} onPress={pickImage}>
        <Text style={styles.imagePickerText}>Upload Image</Text>
      </TouchableOpacity>

      {/* Display Selected Image */}
      {postImage && (
        <Image source={{ uri: postImage }} style={styles.selectedImage} />
      )}

      {/* Submit Button */}
      <TouchableOpacity style={styles.submitButton} onPress={handleSubmitPost}>
        <Text style={styles.submitButtonText}>Post</Text>
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
    backgroundColor: '#28a745',
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