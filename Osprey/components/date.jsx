import React, { useState } from 'react';
import { View, TouchableOpacity, StyleSheet } from 'react-native';
import DateTimePickerModal from 'react-native-modal-datetime-picker'; // Import the modal date picker
import moment from 'moment';
import { Ionicons } from 'react-native-vector-icons';  
import Input from './input';
import { theme } from '../constants/theme';

export default function DatePickerField({ value, onChange }) {
  const [isDatePickerVisible, setDatePickerVisibility] = useState(false);

  // Function to show the date picker modal
  const showDatePicker = () => {
    console.log("Date picker opened"); // Log when the date picker is opened
    setDatePickerVisibility(true);
    console.log("isDatePickerVisible:", true);
  };

  // Function to hide the date picker modal
  const hideDatePicker = () => {
    setDatePickerVisibility(false);
  };

  // Function to handle date selection
  const handleConfirm = (date) => {
    console.log("Selected Date:", date); // Log the selected date
    onChange(date); // Update the parent component state
    hideDatePicker(); // Close the picker
  };

  return (
    <View style={styles.container}>
      {/* Touchable area to open the date picker */}
      <TouchableOpacity style={styles.touchableArea} onPress={showDatePicker}>
        <Input
          icon={<Ionicons name="calendar-number-outline" size={26} color={theme.colors.text} />}
          numberOfLines={1}
          editable={false}
          placeholder="Select Birthdate"
          value={value ? moment(value).format('DD MMMM, YYYY') : ''}
        />
      </TouchableOpacity>

      {/* Date picker modal */}
      <DateTimePickerModal
        isVisible={isDatePickerVisible} // Control visibility
        mode="date" // Set to date picker mode
        onConfirm={handleConfirm} // Handle date selection
        onCancel={hideDatePicker} // Handle cancel
      />
    </View>
  );
}

// Styles
const styles = StyleSheet.create({
  container: {
    marginVertical: 10,
  },
  touchableArea: {
    width: '100%', // Ensure it covers the full width
  },
});