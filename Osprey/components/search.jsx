import React, { useState } from 'react';
import { View, TextInput, FlatList, Text, StyleSheet, TouchableOpacity, ActivityIndicator } from 'react-native';
import { hp, wp } from '../helpers/common'; 
import { theme } from '../constants/theme'; 
import Icon from '../assets/icons'; 
import { router, useRouter } from 'expo-router';


const eventsData = [
  { id: '1', name: 'Music Concert', date: '2023-03-10', location: 'New York' },
  { id: '2', name: 'Art Exhibition', date: '2023-04-15', location: 'Los Angeles' },
  { id: '3', name: 'Tech Conference', date: '2023-05-20', location: 'San Francisco' },
  { id: '4', name: 'Food Festival', date: '2023-06-25', location: 'Chicago' },
  { id: '5', name: 'Fashion Show', date: '2023-07-10', location: 'Miami' },
];

const EventSearch = () => {

  const router = useRouter();
  const [searchQuery, setSearchQuery] = useState(''); 
  const [filteredEvents, setFilteredEvents] = useState(eventsData);
  const [isLoading, setIsLoading] = useState(false); 

  // Filter events based on search query
  const handleSearch = (query) => {
    setSearchQuery(query);
    if (query === '') {
      setFilteredEvents(eventsData); // Reset if query is empty
    } else {
      setIsLoading(true);
      setTimeout(() => {
        const filtered = eventsData.filter(event =>
          event.name.toLowerCase().includes(query.toLowerCase()) ||
          event.location.toLowerCase().includes(query.toLowerCase()) ||
          event.date.includes(query)
        );
        setFilteredEvents(filtered);
        setIsLoading(false);
      }, 500); // Simulate a 0.5s delay to show loading indicator
    }
  };

  // Render each event in the list
  const renderItem = ({ item }) => (
    <View style={styles.eventCard}>
      <Text style={styles.eventName}>{item.name}</Text>
      <Text style={styles.eventDetails}>Date: {item.date}</Text>
      <Text style={styles.eventDetails}>Location: {item.location}</Text>
    </View>
  );

  return (
    <View style={styles.container}>
      {/* Search Input */}
      <View style={styles.searchContainer}>
        <TextInput
          style={styles.searchInput}
          placeholder="Search events"
          value={searchQuery}
          onChangeText={handleSearch}
          clearButtonMode="while-editing" 
        />
        {searchQuery.length > 0 && (
          <TouchableOpacity onPress={() => handleSearch('')}>
            <Icon name="close" size={20} color={theme.colors.primaryDark} style={styles.clearIcon} />
          </TouchableOpacity>
        )}
      </View>

      {/* Loading Indicator */}
      {isLoading ? (
        <ActivityIndicator size="large" color={theme.colors.primary} style={styles.loader} />
      ) : (
        <FlatList
          data={filteredEvents}
          renderItem={renderItem}
          keyExtractor={(item) => item.id}
          ListEmptyComponent={<Text style={styles.noResults}>No events found</Text>}
        />
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: wp(4),
    backgroundColor: theme.colors.background,
  },
  searchContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: hp(3),
    borderRadius: theme.radius.md,
    backgroundColor: 'white',
    paddingHorizontal: wp(4),
    paddingVertical: hp(1),
    elevation: 2, // Shadow for iOS
    shadowColor: theme.colors.dark, // Shadow for Android
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
  },
  searchInput: {
    flex: 1,
    height: hp(6),
    fontSize: hp(2),
    color: theme.colors.text,
    paddingLeft: wp(2),
  },
  clearIcon: {
    marginLeft: wp(2),
  },
  loader: {
    alignSelf: 'center',
    marginTop: hp(4),
  },
  eventCard: {
    padding: wp(4),
    marginBottom: hp(2),
    backgroundColor: theme.colors.cardBackground,
    borderRadius: theme.radius.md,
    shadowColor: theme.colors.dark,
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.2,
    shadowRadius: 6,
    elevation: 2,
  },
  eventName: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
  },
  eventDetails: {
    fontSize: hp(2),
    color: theme.colors.textSecondary,
  },
  noResults: {
    textAlign: 'center',
    fontSize: hp(2),
    color: theme.colors.textSecondary,
    marginTop: hp(5),
  },
})

export default EventSearch;
