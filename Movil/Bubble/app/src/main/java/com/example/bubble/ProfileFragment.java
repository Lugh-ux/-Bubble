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

import com.google.android.material.imageview.ShapeableImageView;


public class ProfileFragment extends Fragment {

    ShapeableImageView imagenPerfil;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_profile, container, false);

        imagenPerfil = view.findViewById(R.id.profileImage);

        imagenPerfil.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(Intent.ACTION_PICK);
                intent.setType("image/*");

                if (getActivity() != null && intent.resolveActivity(getActivity().getPackageManager()) != null) {
                    startActivityForResult(intent, 101);
                } else {
                    startActivityForResult(intent, 101);
                }
            }


        });

        APIRest api = new APIRest();

        SharedPreferences pref = getActivity().getSharedPreferences("Sesion", Context.MODE_PRIVATE);
        String nombreUsuario = pref.getString("username", "Prueba");

        api.descargarImagen(nombreUsuario, imagenPerfil);


        return view;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == 101 && resultCode == android.app.Activity.RESULT_OK && data != null) {
            android.net.Uri imageUri = data.getData();

            android.content.SharedPreferences pref = getActivity().getSharedPreferences("Sesion", android.content.Context.MODE_PRIVATE);
            String nombreUsuario = pref.getString("username", "UsuarioDesconocido");


            imagenPerfil.setImageURI(imageUri);
            APIRest api = new APIRest();
            api.subirImagen(imageUri, getContext(), nombreUsuario);
        }
    }



}