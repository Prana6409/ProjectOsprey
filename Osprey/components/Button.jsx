import { Pressable, StyleSheet, Text, View,ActivityIndicator } from 'react-native'
import React from 'react'
import { theme } from '../constants/theme'
import { hp,wp } from '../helpers/common'

const Button = ({
    buttonStyle,
    textStyle,
    titles,
    onPress=()=>{},
    loading=false,
    hasShadow=true,
}) => {
    const shadowStyle = {
        shadowColor: theme.colors.dark,
        shadowOffset: {width: 0, height: 10},
        shadowOpacity: 0.2,
        shadowRadius: 8,
        elevation: 4
    }
    if (loading){
        return (
            <View style={[styles.button, buttonStyle, {backgroundColor: 'white'}]}>
                <ActivityIndicator size="small" color={theme.colors.primary} />
            </View>
        )
    }


  return (
    <Pressable style={[styles.button, buttonStyle,hasShadow && shadowStyle]} onPress={onPress}>
      <Text style={[styles.text,textStyle]}>{titles}</Text>
    </Pressable>
  )
}

export default Button

const styles = StyleSheet.create({
    button:{
        backgroundColor: theme.colors.primary,
        height: hp(6.6),
        justifyContent: 'center',
        alignItems: 'center',
        borderCurve: 'continuous',
        borderRadius: theme.radius.xl
    },
    text: {
        fontSize: hp(2.5),
        color: 'white',
        fontWeight: theme.fonts.bold
    }
})