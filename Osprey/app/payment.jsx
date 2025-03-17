import React, { useState } from "react";
import { View, Text, TextInput, StyleSheet, Button, Alert, Platform, ScrollView } from "react-native";
import { useRouter } from "expo-router";

const PaymentScreen = () => {
  const router = useRouter();
  const [cardNumber, setCardNumber] = useState("");
  const [expirationDate, setExpirationDate] = useState("");
  const [cvv, setCvv] = useState("");
  const [nameOnCard, setNameOnCard] = useState("");
  const [billingAddress, setBillingAddress] = useState("");
  const [loading, setLoading] = useState(false);

  // Validation for card details
  const validateCardDetails = () => {
    if (!cardNumber || !expirationDate || !cvv || !nameOnCard || !billingAddress) {
      Alert.alert("Error", "Please fill in all the details.");
      return false;
    }

    if (cardNumber.length !== 16) {
      Alert.alert("Error", "Card number must be 16 digits.");
      return false;
    }

    if (cvv.length !== 3) {
      Alert.alert("Error", "CVV must be 3 digits.");
      return false;
    }

    // Expiration date validation (MM/YY)
    const expiryRegex = /^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$/;
    if (!expiryRegex.test(expirationDate)) {
      Alert.alert("Error", "Please enter a valid expiration date (MM/YY).");
      return false;
    }

    return true;
  };

  const onSubmit = () => {
    if (validateCardDetails()) {
      setLoading(true);
      setTimeout(() => {
        setLoading(false);
        Alert.alert("Success", "Payment processed successfully!");
        router.push("/success"); // Navigate to success screen after payment
      }, 2000);
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.header}>Payment Details</Text>

      <TextInput
        style={styles.input}
        placeholder="Card Number"
        keyboardType="numeric"
        maxLength={16}
        value={cardNumber}
        onChangeText={setCardNumber}
      />

      <View style={styles.row}>
        <TextInput
          style={[styles.input, styles.halfInput]}
          placeholder="MM/YY"
          keyboardType="numeric"
          maxLength={5}
          value={expirationDate}
          onChangeText={setExpirationDate}
        />
        <TextInput
          style={[styles.input, styles.halfInput]}
          placeholder="CVV"
          keyboardType="numeric"
          maxLength={3}
          value={cvv}
          onChangeText={setCvv}
        />
      </View>

      <TextInput
        style={styles.input}
        placeholder="Name on Card"
        value={nameOnCard}
        onChangeText={setNameOnCard}
      />

      <TextInput
        style={styles.input}
        placeholder="Billing Address"
        value={billingAddress}
        onChangeText={setBillingAddress}
      />

      <Button title={loading ? "Processing..." : "Submit Payment"} onPress={onSubmit} disabled={loading} />
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flexGrow: 1,
    padding: 20,
  },
  header: {
    fontSize: 24,
    fontWeight: "bold",
    marginBottom: 20,
    textAlign: "center",
  },
  input: {
    height: 50,
    borderColor: "#ccc",
    borderWidth: 1,
    borderRadius: 8,
    marginBottom: 15,
    paddingLeft: 15,
    fontSize: 16,
  },
  row: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  halfInput: {
    width: "48%",
  },
});

export default PaymentScreen;
