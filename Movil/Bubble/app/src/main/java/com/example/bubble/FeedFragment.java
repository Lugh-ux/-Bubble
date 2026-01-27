package com.example.bubble;

import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;
import java.util.List;


public class FeedFragment extends Fragment {

    private RecyclerView recyclerView;
    private List<Actividad> listaActividades;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_feed, container, false);

        recyclerView = view.findViewById(R.id.recyclerFeed);
        recyclerView.setLayoutManager(new LinearLayoutManager(getContext()));


        listaActividades = new ArrayList<>();
        listaActividades.add(new Actividad("Carlos", "creó una burbuja 3D", "A 50m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Marta (Amiga)", "te envió un saludo", "Amigo", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Burbuja Global", "Alguien pasó por Urzáiz", "A 500m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));
        listaActividades.add(new Actividad("Test", "Test", "A 100m", R.drawable.iconobuble));


        ActividadAdapter adapter = new ActividadAdapter(listaActividades);
        recyclerView.setAdapter(adapter);

        return view;
    }
}