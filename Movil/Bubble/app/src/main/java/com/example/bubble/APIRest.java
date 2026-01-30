package com.example.bubble;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.util.Log;

import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.model.CircleOptions;
import com.google.android.gms.maps.model.LatLng;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class APIRest {


    public void subirUsuario(String nombre) {
        new Thread(() -> {

            try {
                URL url = new URL("http://192.130.0.154:8080/tema5maven/rest/deportistas/android");

                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("POST");
                con.setRequestProperty("Content-Type", "application/json");
                con.setDoOutput(true);

                JSONObject json = new JSONObject();
                json.put("nombre", nombre);

                System.out.println(json);

                try (OutputStream os = con.getOutputStream()) {
                    os.write(json.toString().getBytes(StandardCharsets.UTF_8));
                } catch (IOException e ){
                    throw new RuntimeException(e);
                }

                System.out.println(con.getResponseCode());


            } catch (Exception e) {
                throw new RuntimeException(e);
            }

    }).start();
    }



    public void subirBurbuja(double lat, double lng, Activity actividad, GoogleMap mapa) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas/add");
                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("POST");
                con.setRequestProperty("Content-Type", "application/json");
                con.setDoOutput(true);

                JSONObject json = new JSONObject();
                json.put("user_id", 1);
                json.put("latitude", lat);
                json.put("longitude", lng);

                try (OutputStream os = con.getOutputStream()) {
                    os.write(json.toString().getBytes(StandardCharsets.UTF_8));
                }

                if (con.getResponseCode() == 200) {

                    actividad.runOnUiThread(() -> {
                        cargarBurbujasEnMapa(mapa, actividad);
                    });
                }

            } catch (Exception e) {
                e.printStackTrace();
            }
        }).start();
    }

    public void cargarBurbujasEnMapa(GoogleMap mapa, Activity actividad) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas");
                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("GET");

                if (con.getResponseCode() == 200) {
                    java.util.Scanner s = new java.util.Scanner(con.getInputStream()).useDelimiter("\\A");
                    String result = s.hasNext() ? s.next() : "";

                    actividad.runOnUiThread(() -> {
                        try {
                            mapa.clear();

                            JSONArray array = new JSONArray(result);
                            for (int i = 0; i < array.length(); i++) {
                                JSONObject obj = array.getJSONObject(i);

                                double lat = obj.getDouble("latitude");
                                double lon = obj.getDouble("longitude");
                                LatLng pos = new LatLng(lat, lon);

                                mapa.addCircle(new CircleOptions()
                                        .center(pos)
                                        .radius(100)
                                        .strokeWidth(4)
                                        .strokeColor(Color.parseColor("#4285F4")) // Azul Google
                                        .fillColor(Color.parseColor("#404285F4"))); // Azul con transparencia
                            }
                        } catch (Exception e) {
                            Log.e("API_ERROR", "Error parseando JSON: " + e.getMessage());
                        }
                    });
                }
            } catch (Exception e) {
                Log.e("API_ERROR", "Error de conexión: " + e.getMessage());
            }
        }).start();
    }
    public void login(String email, String password, Activity actividad) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/usuarios/login");
                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("POST");
                con.setRequestProperty("Content-Type", "application/json");
                con.setDoOutput(true);

                JSONObject json = new JSONObject();
                json.put("email", email);
                json.put("password", password);

                try (OutputStream os = con.getOutputStream()) {
                    os.write(json.toString().getBytes(StandardCharsets.UTF_8));
                }

                int code = con.getResponseCode();
                if (code == 200) {
                    // Leer el ID que nos devuelve el servidor
                    java.util.Scanner s = new java.util.Scanner(con.getInputStream()).useDelimiter("\\A");
                    JSONObject response = new JSONObject(s.hasNext() ? s.next() : "");
                    long idUsuario = response.getLong("id");

                    // GUARDAR EL ID para usarlo en la burbuja
                    android.content.SharedPreferences pref = actividad.getSharedPreferences("Sesion", android.content.Context.MODE_PRIVATE);
                    pref.edit().putLong("user_id", idUsuario).apply();

                    actividad.runOnUiThread(() -> {
                        actividad.startActivity(new Intent(actividad, Principal.class));
                        actividad.finish();
                    });
                } else {
                    actividad.runOnUiThread(() ->
                            android.widget.Toast.makeText(actividad, "Email o contraseña incorrectos", android.widget.Toast.LENGTH_SHORT).show()
                    );
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }).start();
    }

}
