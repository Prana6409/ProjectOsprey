import React from 'react';
import { View, Text, StyleSheet, Image, ScrollView, TouchableOpacity } from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { wp, hp } from '../helpers/common'; // Adjust to your helper for responsive scaling
import { theme } from '../constants/theme';
import { useRouter } from 'expo-router';
import Icon from 'react-native-vector-icons/FontAwesome';
import Header from '../components/Header';  

const AboutUs = () => {
  const router = useRouter();

  return (
    <ScrollView style={styles.container}>
      <StatusBar style="dark" />
      <Header router={router}/>
      
      {/* Header Section */}
      <View style={styles.headerContainer}>
        <Text style={styles.pageTitle}>About Us</Text>
        <Text style={styles.pageSubtitle}>Empowering Young Females with Osprey</Text>
      </View>

      {/* Mission Statement */}
      <View style={styles.missionContainer}>
        <Text style={styles.missionTitle}>Our Mission</Text>
        <Text style={styles.missionDescription}>
          At Osprey, we aim to empower young females by providing a platform to grow, connect, and share their creative projects. We believe in fostering a supportive environment where young women can develop their skills and build meaningful connections to succeed in life and beyond.
        </Text>
      </View>

      {/* Visual Section */}
      <Image
        source={require('../assets/images/about.jpg')}
        style={styles.missionImage}
      />

      {/* Core Values Section */}
      <View style={styles.valuesContainer}>
        <Text style={styles.valuesTitle}>Our Core Values</Text>
        <View style={styles.valuesList}>
          <View style={styles.valueItem}>
            <Icon name="heart" size={30} color={theme.colors.primaryDark} style={styles.valueIcon} />
            <Text style={styles.valuesDescription}>Empowerment: We empower young women to take charge of their futures.</Text>
          </View>
          <View style={styles.valueItem}>
            <Icon name="users" size={30} color={theme.colors.primaryDark} style={styles.valueIcon} />
            <Text style={styles.valuesDescription}>Community: We foster a community of like-minded individuals who support and encourage each other.</Text>
          </View>
          <View style={styles.valueItem}>
            <Icon name="rocket" size={30} color={theme.colors.primaryDark} style={styles.valueIcon} />
            <Text style={styles.valuesDescription}>Growth: We provide resources and tools to help young females grow personally and professionally.</Text>
          </View>
        </View>
      </View>

      {/* Join Us Section */}
      <View style={styles.joinUsContainer}>
        <Text style={styles.joinUsTitle}>Join Us Today!</Text>
        <Text style={styles.joinUsDescription}>
          Become part of the Osprey community today. We are looking for passionate, driven individuals who want to make a difference in the world. Together, we can achieve greatness!
        </Text>
        <TouchableOpacity style={styles.button} onPress={() => router.push('/signUp')}>
          <Text style={styles.buttonText}>Sign Up</Text>
        </TouchableOpacity>
      </View>

      {/* Footer Section */}
      <View style={styles.footerContainer}>
        <Text style={styles.footerText}>Want to Learn More?</Text>
        <TouchableOpacity onPress={() => router.push('https://www.joinosprey.com')}>
          <Text style={[styles.footerText, { color: theme.colors.primaryDark, fontWeight: theme.fonts.semibold }]}>
            Visit our website
          </Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
    paddingHorizontal: wp(5),
    paddingTop: hp(3),
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    marginTop: hp(2),
    backgroundColor: theme.colors.white,
  },
  headerContainer: {
    marginBottom: hp(3),
    textAlign: 'center',
    backgroundColor: 'gold',
    padding: hp(2),
    borderRadius: 10,
    marginBottom: hp(4),
    elevation: 5, // Shadow for Android
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.1,
    shadowRadius: 15,
  },
  pageTitle: {
    fontSize: hp(4),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
    textAlign: 'center',
  },
  pageSubtitle: {
    fontSize: hp(2),
    color: theme.colors.secondaryText,
    textAlign: 'center',
    marginTop: hp(1),
  },
  missionContainer: {
    marginVertical: hp(3),
  },
  missionTitle: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
    marginBottom: hp(1),
  },
  missionDescription: {
    fontSize: hp(1.8),
    color: theme.colors.text,
    marginTop: hp(1),
    lineHeight: 25,
  },
  missionImage: {
    width: '100%',
    height: hp(30),
    
    marginTop: hp(2),
    marginBottom: hp(2),
  },
  valuesContainer: {
    marginVertical: hp(3),
  },
  valuesTitle: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.bold,
    color: theme.colors.text,
    marginBottom: hp(1),
  },
  valuesList: {
    marginTop: hp(2),
  },
  valueItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: hp(2),
  },
  valueIcon: {
    marginRight: wp(3),
  },
  valuesDescription: {
    fontSize: hp(1.7),
    color: theme.colors.text,
    flexShrink: 1,
    marginBottom: hp(1),
  },
  joinUsContainer: {
    marginVertical: hp(3),
    alignItems: 'center',
    backgroundColor:'gold',
    padding: hp(3),
    borderRadius: 10,
    elevation: 5,
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.1,
    shadowRadius: 15,
  },
  joinUsTitle: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.bold,
    color: theme.colors.white,
  },
  joinUsDescription: {
    fontSize: hp(1.8),
    color: theme.colors.white,
    marginVertical: hp(2),
    textAlign: 'center',
  },
  button: {
    backgroundColor: theme.colors.primaryDark,
    paddingVertical: hp(1.2),
    paddingHorizontal: wp(8),
    borderRadius: 30,
    marginTop: hp(2),
  },
  buttonText: {
    color: 'white',
    fontSize: hp(1.8),
    fontWeight: theme.fonts.semibold,
    textAlign: 'center',
  },
  footerContainer: {
    alignItems: 'center',
    marginTop: hp(4),
    marginBottom: hp(5),
    
  },
  footerText: {
    fontSize: hp(1.6),
    color: theme.colors.text,
    textAlign: 'center',
  },
});

export default AboutUs;