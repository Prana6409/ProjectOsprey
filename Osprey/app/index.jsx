import React, { useState, useEffect, useRef } from 'react';
import { View, Text,Image, StyleSheet, ScrollView, TouchableOpacity, Dimensions } from 'react-native';
import { router } from 'expo-router';
import { wp, hp } from '../helpers/common'; // Adjust to your helper for responsive scaling
import { theme } from '../constants/theme';
import ScreenWrapper from '../components/ScreenWrapper';
import Icon from 'react-native-vector-icons/FontAwesome'; // Importing the icon library
import Button from '../components/Button';


// Feature data for the slides
const featureData = [
  {
    title: 'Connect',
    description: 'Join a community of like-minded individuals.',
    icon: 'users', // FontAwesome icon name for "users"
  },
  {
    title: 'Grow',
    description: 'Develop your skills with our tools and resources.',
    icon: 'rocket', // FontAwesome icon name for "rocket"
  },
  {
    title: 'Share',
    description: 'Collaborate and share your projects with the world.',
    icon: 'share-alt', // FontAwesome icon name for "share"
  },
];

const { width } = Dimensions.get('window');

const Welcome = () => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const scrollViewRef = useRef(null);

  // Automatically change slide every 3 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      const nextIndex = (currentIndex + 1) % featureData.length;
      setCurrentIndex(nextIndex);

      // Scroll to the next slide
      if (scrollViewRef.current) {
        scrollViewRef.current.scrollTo({ x: nextIndex * width, animated: true });
      }
    }, 3000); // Change slide every 3 seconds

    // Clean up interval on component unmount
    return () => clearInterval(interval);
  }, [currentIndex]);

  return (

      <View style={styles.container}>
        {/* Welcome Image */}
            <Image style={styles.welcomeImage} source={require('../assets/images/homeImage.jpg')}/>

        {/* Title and Punchline */}
        <View style={styles.titleContainer}>
          <Text style={styles.title}>Osprey</Text>
          <Text style={styles.punchline}>Empowering young females!!!</Text>
        </View>

        {/* Feature Slideshow Section */}
        <View style={styles.slideshowContainer}>
          <ScrollView
            ref={scrollViewRef}
            horizontal
            pagingEnabled
            showsHorizontalScrollIndicator={false}
            scrollEnabled={true} // Disable manual scroll, rely on auto-transition
            contentContainerStyle={styles.slidesContainer}
          >
            {featureData.map((item, index) => (
              <View key={index} style={[styles.slide, { width }]}>
                <Icon name={item.icon} size={50} color={theme.colors.primaryDark} style={styles.featureIcon} />
                <Text style={styles.featureTitle}>{item.title}</Text>
                <Text style={styles.featureDescription}>{item.description}</Text>
              </View>
            ))}
          </ScrollView>
        </View>

        {/* Footer (Get Started / Login) */}
        <View style={styles.footer}>
          <Button
            titles={'Get Started'}
            buttonStyle={styles.buttonStyle}
            onPress={() => router.push('/welcome2')}
          />
          <Button
            titles={'buttons'}
            buttonStyle={styles.buttonStyle}
            onPress={() => router.push('/pressables')}
          />
          <View style={styles.bottomTextContainer}>
            <Text style={styles.loginText}>Already have an account?</Text>
            <TouchableOpacity onPress={() => router.push('/login')}>
              <Text style={[styles.loginText, { color: theme.colors.primaryDark, fontWeight: theme.fonts.semibold }]}>
                Login
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-around',
    alignItems: 'center',
    backgroundColor: 'black',
    paddingHorizontal: wp(10),
  },

  welcomeImage: {
    height:230,
    width:width,
    color: theme.colors.primaryDark,
    marginBottom: hp(1),
  },

  titleContainer: {
    alignItems: 'center',
    marginBottom: hp(2),
    
  },
  title: {
    fontSize: hp(4),
    fontWeight: theme.fonts.extrabold,
    color: theme.colors.text,
    textAlign: 'center',
    color:'white'
  },
  punchline: {
    fontSize: hp(1.7),
    color: theme.colors.text,
    textAlign: 'center',
    paddingHorizontal: wp(5),
    color:'white',
  },

  // Slideshow Styles
  slideshowContainer: {
    width: width,
    marginTop: hp(1),
    overflow: 'hidden', // Ensures content doesn't spill out
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.2,
    shadowRadius: 6,
    elevation: 3,
    alignItems:'center', // Adds a shadow for Android
    backgroundColor:'gold',
  },

  slidesContainer: {
   

    justifyContent: 'center', 
    alignItems: 'center', 
  },

  slide: {
    width: width, // Use the full width of the screen
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 10,
    overflow: 'hidden',
    backgroundColor: 'gold',
    padding: hp(2),
  },

  featureIcon: {
    marginBottom: hp(2),
  },
  featureTitle: {
    fontSize: hp(2.5),
    fontWeight: theme.fonts.extrabold,
    textAlign: 'center',
    marginTop: hp(1),
    color: theme.colors.text,
  },
  featureDescription: {
    fontSize: hp(1.6),
    textAlign: 'center',
    marginTop: hp(1),
    color: theme.colors.text,
  },

  footer: {
    width: '100%',
    gap: 30,
  },

  buttonStyle: {
    marginHorizontal: wp(3),
  },

  bottomTextContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    gap: 5,
  },
  loginText: {
    fontSize: hp(1.6),
    color: 'white',
    fontWeight: theme.fonts.bold,
    textAlign: 'center',
  },
});

export default Welcome;
