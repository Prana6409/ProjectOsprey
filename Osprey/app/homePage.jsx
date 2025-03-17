import React, { useState } from 'react';
import { View, Text, Image, StyleSheet, FlatList, TouchableOpacity, TextInput, Alert } from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { wp, hp } from '../helpers/common'; // Adjust to your helper for responsive scaling
import { theme } from '../constants/theme';
import Icon from 'react-native-vector-icons/FontAwesome';
import { useRouter } from 'expo-router';
import ScreenWrapper from '../components/ScreenWrapper';
import Header from '../components/Header';
import axios from 'axios'; // Import axios for API calls

const MainPage = () => {
  const router = useRouter();

  // State for search query, filtered events, and loading
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredEvents, setFilteredEvents] = useState([]);
  const [loading, setLoading] = useState(false);

  // Function to handle search-as-you-type
  const handleSearchAsYouType = async (query) => {
    setSearchQuery(query);

    if (query.trim() === '') {
      setFilteredEvents([]); // Clear results if the query is empty
      return;
    }

    setLoading(true);

    try {
      // Make API call to search usernames with partial match
      const response = await axios.get(`/api/function/search-usernames`, {
        params: {
          query: query,
          exactMatch: false, // Partial match for search-as-you-type
        },
      });

      if (response.data && response.data.length > 0) {
        // Map the API response to the event list format
        const mappedEvents = response.data.map((item) => ({
          id: item.Data.UqID, // Use UqID as the unique identifier
          image: item.Data.ProfilePictureUrl || 'https://placehold.co/600x400', // Default image if no profile picture
          title: item.Data.Username,
          description: `Role: ${item.Collection}`, // Display the role (e.g., User, Sportsman, etc.)
          user: item.Data.Username,
          likes: 0, // Placeholder for likes
          comments: 0, // Placeholder for comments
        }));

        setFilteredEvents(mappedEvents);
      } else {
        setFilteredEvents([]); // No results found
      }
    } catch (error) {
      console.error('Search Error:', error);
      Alert.alert('Error', 'Something went wrong. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  // Function to handle exact match search
  const handleExactMatchSearch = async () => {
    if (searchQuery.trim() === '') {
      Alert.alert('Error', 'Please enter a search query.');
      return;
    }

    setLoading(true);

    try {
      // Make API call to search usernames with exact match
      const response = await axios.get(`/api/function/search-usernames`, {
        params: {
          query: searchQuery,
          exactMatch: true, // Exact match for search button click
        },
      });

      if (response.data && response.data.length > 0) {
        // Map the API response to the event list format
        const mappedEvents = response.data.map((item) => ({
          id: item.Data.UqID, // Use UqID as the unique identifier
          image: item.Data.ProfilePictureUrl || 'https://placehold.co/600x400', // Default image if no profile picture
          title: item.Data.Username,
          description: `Role: ${item.Collection}`, // Display the role (e.g., User, Sportsman, etc.)
          user: item.Data.Username,
          likes: 0, // Placeholder for likes
          comments: 0, // Placeholder for comments
        }));

        setFilteredEvents(mappedEvents);
      } else {
        Alert.alert('No Results', `No exact matches found for '${searchQuery}'.`);
        setFilteredEvents([]); // No results found
      }
    } catch (error) {
      console.error('Search Error:', error);
      Alert.alert('Error', 'Something went wrong. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  // Render each event item
  const renderEventItem = ({ item }) => (
    <View style={styles.card}>
      {/* Event Image */}
      <Image source={{ uri: item.image }} style={styles.eventImage} />

      <View style={styles.cardContent}>
        {/* Title and User */}
        <Text style={styles.eventTitle}>{item.title}</Text>
        <Text style={styles.eventUser}>Hosted by {item.user}</Text>

        {/* Event Description */}
        <Text style={styles.eventDescription}>{item.description}</Text>

        {/* Action Buttons */}
        <View style={styles.actionContainer}>
          <TouchableOpacity style={styles.actionButton}>
            <Icon name="heart" size={24} color={theme.colors.primaryDark} />
            <Text style={styles.actionText}>{item.likes}</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionButton}>
            <Icon name="comment" size={24} color={theme.colors.primaryDark} />
            <Text style={styles.actionText}>{item.comments}</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionButton}>
            <Icon name="share" size={24} color={theme.colors.primaryDark} />
          </TouchableOpacity>
        </View>
      </View>
    </View>
  );

  return (
    <ScreenWrapper>
      <Header router={router} />
      <View style={styles.container}>
        <StatusBar style="dark" />

        {/* Search Bar */}
        <View style={styles.searchContainer}>
          <TextInput
            style={styles.searchInput}
            placeholder="Search for users"
            value={searchQuery}
            onChangeText={handleSearchAsYouType} // Search-as-you-type
            placeholderTextColor={theme.colors.secondaryText}
          />
          <TouchableOpacity onPress={handleExactMatchSearch} style={styles.searchButton}>
            <Icon name="search" size={24} color={theme.colors.primaryDark} />
          </TouchableOpacity>
        </View>

        {/* Event List */}
        {loading ? (
          <Text style={styles.loadingText}>Loading...</Text>
        ) : filteredEvents.length > 0 ? (
          <FlatList
            data={filteredEvents}
            renderItem={renderEventItem}
            keyExtractor={(item) => item.id}
            contentContainerStyle={styles.list}
          />
        ) : (
          <Text style={styles.noResultsText}>No results found.</Text>
        )}
      </View>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
    paddingHorizontal: wp(5),
    paddingTop: hp(2),
  },
  list: {
    paddingBottom: hp(10), // Space at the bottom to avoid content being cut off
  },
  card: {
    backgroundColor: 'white',
    borderRadius: 10,
    overflow: 'hidden',
    marginBottom: hp(2),
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.25,
    shadowRadius: 3.5,
    elevation: 5, // For Android shadow
  },
  eventImage: {
    width: '100%',
    height: hp(30),
    borderTopLeftRadius: 10,
    borderTopRightRadius: 10,
    resizeMode: 'cover',
  },
  cardContent: {
    padding: wp(4),
  },
  eventTitle: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  eventUser: {
    fontSize: hp(1.8),
    color: theme.colors.secondaryText,
    marginVertical: hp(1),
  },
  eventDescription: {
    fontSize: hp(1.6),
    color: theme.colors.text,
    marginVertical: hp(1),
  },
  actionContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: hp(2),
  },
  actionButton: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: wp(2),
  },
  actionText: {
    fontSize: hp(1.6),
    color: theme.colors.text,
  },
  searchContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: hp(3),
    marginTop: hp(1),
    paddingHorizontal: wp(4),
    backgroundColor: theme.colors.backgroundLight,
    borderRadius: theme.radius.xl,
    paddingVertical: hp(1.5),
  },
  searchInput: {
    flex: 1,
    fontSize: hp(1.8),
    color: theme.colors.text,
    paddingLeft: wp(4),
  },
  searchButton: {
    padding: wp(2),
  },
  loadingText: {
    textAlign: 'center',
    marginTop: hp(5),
    fontSize: hp(2),
    color: theme.colors.text,
  },
  noResultsText: {
    textAlign: 'center',
    marginTop: hp(5),
    fontSize: hp(2),
    color: theme.colors.secondaryText,
  },
});

export default MainPage;