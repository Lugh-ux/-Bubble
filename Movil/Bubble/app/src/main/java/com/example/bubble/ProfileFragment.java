package com.example.bubble;

import static android.app.Activity.RESULT_OK;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import com.google.android.material.imageview.ShapeableImageView;


public class ProfileFragment extends Fragment {

    ShapeableImageView imagenPerfil;
    TextView txtNombre, txtBio, txtBurbujas, txtDistancia;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_profile, container, false);

        imagenPerfil = view.findViewById(R.id.profileImage);
        txtNombre = view.findViewById(R.id.textView6);
        txtBio = view.findViewById(R.id.txtBio);
        txtBurbujas = view.findViewById(R.id.txtBurbujasCount);
        txtDistancia = view.findViewById(R.id.txtDistanciaCount);
        SharedPreferences pref = getActivity().getSharedPreferences("Sesion", Context.MODE_PRIVATE);
        String nombreUsuario = pref.getString("username", "Usuario");
        long userId = pref.getLong("user_id", -1);

        txtNombre.setText("Perfil de: " + nombreUsuario);

        APIRest api = new APIRest();
        api.descargarImagen(nombreUsuario, imagenPerfil);



        imagenPerfil.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_PICK);
            intent.setType("image/*");
            startActivityForResult(intent, 101);
        });

        api.cargarStats(userId, txtBurbujas, txtDistancia, getActivity());

        return view;
    }



    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == 101 && resultCode == android.app.Activity.RESULT_OK && data != null) {
            android.net.Uri imageUri = data.getData();
            SharedPreferences pref = getActivity().getSharedPreferences("Sesion", android.content.Context.MODE_PRIVATE);
            String nombreUsuario = pref.getString("username", "UsuarioDesconocido");

            imagenPerfil.setImageURI(imageUri);
            APIRest api = new APIRest();
            api.subirImagen(imageUri, getContext(), nombreUsuario);
        }
    }
}
