package com.example.bubble;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.core.app.ActivityCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;

import org.json.JSONArray;
import org.json.JSONObject;

import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class FeedFragment extends Fragment {

    private RecyclerView recyclerFeed;
    private List<Actividad> listaActividades;
    private ActividadAdapter adapterFeed;

    private RecyclerView recyclerRadar;
    private List<BurbujaRadar> listaRadar;
    private RadarAdapter adapterRadar;

    private FusedLocationProviderClient fusedLocationClient;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_feed, container, false);

        recyclerFeed = view.findViewById(R.id.rvNotifications);
        recyclerFeed.setLayoutManager(new LinearLayoutManager(getContext()));
        listaActividades = new ArrayList<>();
        adapterFeed = new ActividadAdapter(listaActividades);
        recyclerFeed.setAdapter(adapterFeed);

        recyclerRadar = view.findViewById(R.id.rvRadar);
        recyclerRadar.setLayoutManager(new LinearLayoutManager(getContext()));
        listaRadar = new ArrayList<>();
        adapterRadar = new RadarAdapter(listaRadar);
        recyclerRadar.setAdapter(adapterRadar);

        fusedLocationClient = LocationServices.getFusedLocationProviderClient(requireActivity());


        cargarNotificaciones();
        cargarRadar();

        return view;
    }

    private void cargarNotificaciones() {
        if (getActivity() == null) return;

        int userId = (int) getActivity().getSharedPreferences("Sesion", Context.MODE_PRIVATE)
                .getLong("user_id", -1);

        if (userId == -1) {
            Toast.makeText(getContext(), "Usuario no identificado", Toast.LENGTH_SHORT).show();
            return;
        }

        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas/notificaciones/" + userId);
                HttpURLConnection conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("GET");
                conn.setConnectTimeout(5000);

                if (conn.getResponseCode() == 200) {
                    Scanner scanner = new Scanner(conn.getInputStream());
                    StringBuilder response = new StringBuilder();
                    while (scanner.hasNext()) {
                        response.append(scanner.nextLine());
                    }
                    scanner.close();

                    JSONArray jsonArray = new JSONArray(response.toString());
                    List<Actividad> listaNueva = new ArrayList<>();

                    for (int i = 0; i < jsonArray.length(); i++) {
                        JSONObject obj = jsonArray.getJSONObject(i);
                        listaNueva.add(new Actividad(
                                obj.getString("titulo"),
                                obj.getString("mensaje"),
                                "Ahora",
                                R.drawable.iconobuble
                        ));
                    }

                    if (isAdded() && getActivity() != null) {
                        getActivity().runOnUiThread(() -> {
                            listaActividades.clear();
                            listaActividades.addAll(listaNueva);
                            adapterFeed.notifyDataSetChanged();
                        });
                    }
                } else {
                    mostrarError("Error del servidor (Notis): " + conn.getResponseCode());
                }

            } catch (Exception e) {
                e.printStackTrace();
                mostrarError("Error de red: " + e.getMessage());
            }
        }).start();
    }

    private void cargarRadar() {
        if (getActivity() == null) return;

        if (ActivityCompat.checkSelfPermission(getContext(), Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            System.out.println("RADAR: Permisos OK. Pidiendo ubicación al GPS...");

            fusedLocationClient.getLastLocation().addOnSuccessListener(location -> {
                if (location != null) {
                    System.out.println("RADAR: ¡Ubicación encontrada! Lat: " + location.getLatitude() + " Lon: " + location.getLongitude());
                    llamarApiRadar(location.getLatitude(), location.getLongitude());
                } else {
                    System.out.println("RADAR: ERROR - La ubicación es NULL.");
                    mostrarError("Esperando señal GPS... Ve al Home un segundo y vuelve.");
                }
            }).addOnFailureListener(e -> {
                System.out.println("RADAR: ERROR al pedir ubicación - " + e.getMessage());
            });
        } else {
            System.out.println("RADAR: ERROR - No se tienen permisos de ubicación en esta pantalla.");
        }
    }

    private void llamarApiRadar(double lat, double lon) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas/radar?lat=" + lat + "&lon=" + lon);
                HttpURLConnection conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("GET");
                conn.setConnectTimeout(5000);

                if (conn.getResponseCode() == 200) {
                    Scanner scanner = new Scanner(conn.getInputStream());
                    StringBuilder response = new StringBuilder();
                    while (scanner.hasNext()) { response.append(scanner.nextLine()); }
                    scanner.close();

                    JSONArray jsonArray = new JSONArray(response.toString());
                    List<BurbujaRadar> listaNuevaRadar = new ArrayList<>();

                    for (int i = 0; i < jsonArray.length(); i++) {
                        JSONObject obj = jsonArray.getJSONObject(i);

                        String mensaje = obj.optString("mensaje", "Burbuja comun");
                        String distancia = obj.getString("distancia");

                        listaNuevaRadar.add(new BurbujaRadar(mensaje, distancia));
                    }

                    if (isAdded() && getActivity() != null) {
                        getActivity().runOnUiThread(() -> {
                            listaRadar.clear();
                            listaRadar.addAll(listaNuevaRadar);
                            adapterRadar.notifyDataSetChanged();
                        });
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }).start();
    }

    private void mostrarError(String mensaje) {
        if (isAdded() && getActivity() != null) {
            getActivity().runOnUiThread(() ->
                    Toast.makeText(getContext(), mensaje, Toast.LENGTH_SHORT).show()
            );
        }
    }
}