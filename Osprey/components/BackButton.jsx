import { Pressable, StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { theme } from '../constants/theme'
import Icon from '../assets/icons'
import { useRouter } from 'expo-router'


const BackButton = ({size=26}) => {
    const router = useRouter();
  return (
    <Pressable onPress={() => router.back()} style={{padding:10}}>
        <Icon name="arrowLeft" size={size} strokeWidth={2.5} color={theme.colors.text}/>

    </Pressable>
  )
}

export default BackButton

const styles = StyleSheet.create({
button:{
    alignSelf:'flex-start',
    padding:5,
    borderRadius:theme.radius.sm,
    backgroundColor:'rgba(0,0,0,0.07)'
}

})