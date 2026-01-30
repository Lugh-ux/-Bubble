package com.example.bubble;

import android.Manifest;
import android.content.pm.PackageManager;
import android.location.Location; // NUEVO
import android.os.Bundle;
import androidx.annotation.NonNull;
import androidx.core.app.ActivityCompat;
import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.google.android.gms.location.LocationRequest; // DEBE SER ESTE
import com.google.android.gms.location.LocationCallback;

import com.google.android.gms.location.LocationServices;


import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.maps.model.Circle;
import com.google.android.gms.maps.model.CircleOptions;
import android.graphics.Color;


import com.google.android.gms.location.Priority;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.MapView;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.model.BitmapDescriptorFactory;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;

public class HomeFragment extends Fragment implements OnMapReadyCallback {

    private MapView mapView;
    private GoogleMap mMap;
    private FusedLocationProviderClient fusedLocationClient;
    private Marker marcadorUsuario;
    private LocationRequest locationRequest;
    private LocationCallback locationCallback;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home, container, false);

        fusedLocationClient = LocationServices.getFusedLocationProviderClient(requireActivity());


        locationRequest = new LocationRequest.Builder(Priority.PRIORITY_HIGH_ACCURACY, 1000)
                .setMinUpdateIntervalMillis(1000)
                .build();

        locationCallback = new com.google.android.gms.location.LocationCallback() {
            @Override
            public void onLocationResult(com.google.android.gms.location.LocationResult locationResult) {
                if (locationResult == null) return;
                for (android.location.Location location : locationResult.getLocations()) {

                    actualizarMarcadorYDireccion(location);
                }
            }
        };

        mapView = view.findViewById(R.id.mapView);
        mapView.onCreate(savedInstanceState);
        mapView.getMapAsync(this);

        return view;
    }

    @Override
    public void onMapReady(@NonNull GoogleMap googleMap) {
        mMap = googleMap;

        if (ActivityCompat.checkSelfPermission(getContext(), Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            mMap.setMyLocationEnabled(true);

            centrarCamaraAlInicio();
        } else {

            ActivityCompat.requestPermissions(getActivity(), new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        }
        APIRest api = new APIRest();
        api.cargarBurbujasEnMapa(mMap, getActivity());
    }



    public void crearBurbuja() {
        if (mMap == null) return;

        if (ActivityCompat.checkSelfPermission(getContext(), Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            fusedLocationClient.getLastLocation().addOnSuccessListener(location -> {
                if (location != null) {
                    LatLng miLugar = new LatLng(location.getLatitude(), location.getLongitude());

                    mMap.addCircle(new CircleOptions()
                            .center(miLugar)
                            .radius(100)
                            .strokeWidth(4)
                            .strokeColor(Color.parseColor("#4285F4"))
                            .fillColor(Color.parseColor("#404285F4"))
                            .zIndex(1));


                    mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(miLugar, 16f));

                    APIRest api = new APIRest();
                    api.subirBurbuja(location.getLatitude(), location.getLongitude(),this.getActivity(),mMap);

                    Toast.makeText(getContext(), "Burbuja guardada en el servidor", Toast.LENGTH_SHORT).show();

                }
            });
        }
    }

    private void centrarCamaraAlInicio() {
        if (ActivityCompat.checkSelfPermission(getContext(), Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) return;

        fusedLocationClient.getLastLocation().addOnSuccessListener(location -> {
            if (location != null) {
                LatLng miUbicacion = new LatLng(location.getLatitude(), location.getLongitude());


                CameraPosition cameraPosition = new CameraPosition.Builder()
                        .target(miUbicacion)
                        .zoom(17)
                        .tilt(55)
                        .build();

                mMap.moveCamera(CameraUpdateFactory.newCameraPosition(cameraPosition));
            }
        });
    }

    private void actualizarMarcadorYDireccion(Location location) {
        LatLng latLng = new LatLng(location.getLatitude(), location.getLongitude());

        if (marcadorUsuario == null) {
            marcadorUsuario = mMap.addMarker(new MarkerOptions()
                    .position(latLng)
                    .title("Tú")
                    .flat(true)
                    .anchor(0.5f, 0.5f)
                    .icon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_AZURE)));
        } else {
            marcadorUsuario.setPosition(latLng);
        }

        if (location.hasBearing()) {
            marcadorUsuario.setRotation(location.getBearing());
        }

        mMap.animateCamera(CameraUpdateFactory.newLatLng(latLng));
    }




    @Override
    public void onResume() {
        super.onResume();
        startLocationUpdates();
    }

    @Override
    public void onPause() {
        super.onPause();
        fusedLocationClient.removeLocationUpdates(locationCallback);
    }

    private void startLocationUpdates() {
        if (ActivityCompat.checkSelfPermission(getContext(), Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            fusedLocationClient.requestLocationUpdates(locationRequest, locationCallback, null);
        }
    }

    @Override
    public void onDestroy() { super.onDestroy(); if (mapView != null) mapView.onDestroy(); }

    @Override
    public void onLowMemory() { super.onLowMemory(); if (mapView != null) mapView.onLowMemory(); }


}