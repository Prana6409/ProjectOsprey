import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { useRouter } from 'expo-router';
import Button from '../components/Button';
import { hp } from '../helpers/common';
import ScreenWrapper from '../components/ScreenWrapper';
import BackButton from '../components/BackButton';

const UserTypeSelection = () => {
  const router = useRouter();

  const navigateToGeneral = () => {
    router.push('/signUp/general'); 
  };

  const navigateToMentor = () => {
    router.push('/signUp/mentor'); 
  };

  const navigateToSponser = () => {
    router.push('/signUp/sponser'); 
  };

 

  return (
    <ScreenWrapper>
      <BackButton router={router}/>
    <View style={styles.container}>
      <Text style={styles.header}>Select Your User Type</Text>

      <Button
        titles={'General Users'}
        style={styles.button}
        onPress={navigateToGeneral}
      >
        
      </Button>

      <Button
        titles={'Sponser'}
        style={styles.button}
        onPress={navigateToSponser}
      />
        

      <Button
      titles={'Mentors'}
        style={styles.button}
        onPress={navigateToMentor}
      />
        

      
    </View>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: {
    marginTop:hp(15),
    flex: 1,
    gap:15,
    
    paddingHorizontal: 20,
  },
  header: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 30,
  },
  button: {
    width: '100%',
    padding: 15,
    backgroundColor: '#0066cc',
    marginVertical: 10,
    borderRadius: 8,
    alignItems: 'center',
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
  },
});

export default UserTypeSelection;