import { Pressable, StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { useRouter } from 'expo-router'

const Pressablesss = () => {
    const router= useRouter();
  return (
    <View>
     <Pressable onPress={() => router.push ('/payment')}>
       <Text>pay</Text> 
     </Pressable>
     <Pressable onPress={() => router.push('/date')}>
      <Text>genreal</Text>
     </Pressable>
    </View>
  )
}

export default Pressablesss

