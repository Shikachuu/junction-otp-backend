package junctionx.hackathon.otp

import android.Manifest
import android.content.pm.PackageManager
import android.location.Location
import android.location.LocationListener
import android.location.LocationManager
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat

import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.SupportMapFragment
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.MarkerOptions
import org.json.JSONArray

import java.net.URL
import java.util.*


class MapsActivity : AppCompatActivity(), OnMapReadyCallback {

    private lateinit var mMap: GoogleMap

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_maps)
        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        val mapFragment = supportFragmentManager
                .findFragmentById(R.id.map) as SupportMapFragment
        mapFragment.getMapAsync(this)
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */


    class AtmInfo(val postion: LatLng) {
        public var estimatedTimeInMinutes: Double = 0.0;
        public var instructionText: String = "";
    }


    fun FetchDataForDestination(destination: String): Vector<AtmInfo> {
        // http://localhost:5000/atm?origin=47.475828,19.099312&destination=47.510687,19.055810

        val currentLocationAsString = ConvertLatlongToString(currentLocation!!)
        var ip = "100.98.11.34"
        var url = "http://$ip:5000/atm?origin=$destination&destination=$currentLocationAsString"


        var jsonAsString = URL(url).readText()

        //var rootObject = JSONObject(jsonAsString)

        var atms = Vector<AtmInfo>()

        val rootArray = JSONArray(jsonAsString)
        for (i in 0 until rootArray.length()) {
            val item = rootArray.getJSONObject(i)
            val totalTravelTime = item.getDouble("totalTravelTime")
            val instructions1 = item.getJSONObject("routeFromDepartureToAtm").optJSONObject("userInstructionsForRoute")
            val instructions2 = item.getJSONObject("routeFromAtmToDestination").optJSONObject("userInstructionsForRoute")

            val instructions = "${instructions1}\nUse ATM\n${instructions2}"


            var posAsString = item.getJSONObject("atm").getString("atmPosition")
            posAsString = posAsString.substringAfter("(").substringBefore(")")


            val latlong =  posAsString.split(" ");
            val latitude = latlong[0].toDouble();
            val longitude = latlong[1].toDouble();
            val position = LatLng(latitude, longitude)

            var atmInfo = AtmInfo(position)
            atmInfo.estimatedTimeInMinutes = totalTravelTime
            atmInfo.instructionText = instructions

            atms.addElement(atmInfo)
        }
        return atms
    }

    fun ConvertLatlongToString(it: LatLng):String {
        return it.latitude.toString() + "," + it.longitude.toString()
    }

    override fun onMapReady(googleMap: GoogleMap) {
        mMap = googleMap

        // Add a marker in Sydney and move the camera
//        val sydney = LatLng(-34.0, 151.0)
//        mMap.addMarker(MarkerOptions().position(sydney).title("Marker in Sydney"))
//        mMap.moveCamera(CameraUpdateFactory.newLatLng(sydney))

        mMap.setOnMapClickListener {
            var postitionAsString = ConvertLatlongToString(it)
            Thread {
                val atms = FetchDataForDestination(postitionAsString)
                runOnUiThread {
                    onAtmsFetched(atms)
                }
            }

        }

        getLocation()

//        google.maps.event.addListener(map, 'click', function(event) {
//            placeMarker(event.latLng);
//        });
    }

    private fun onAtmsFetched(atms: Vector<MapsActivity.AtmInfo>) {
        atms.forEach {
            var markerOptinons = MarkerOptions()

            mMap.addMarker(MarkerOptions().position(it.postion).title("Marker in Sydney"))
        }
    }

    var locationManager : LocationManager? = null
    var currentLocationAsString: String? = null

    fun getLocation() {

        locationManager = getSystemService(LOCATION_SERVICE) as LocationManager?

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(
                    this,
                    arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
                    PERMISSION_REQUEST_ACCESS_FINE_LOCATION)
            return
        }

        locationManager!!.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 0L, 0f, locationListener)
    }

    var currentLocation : LatLng? = null

    var locationListener = object : LocationListener{
        override fun onLocationChanged(location: Location?) {
            var latitute = location!!.latitude
            var longitute = location!!.longitude

            val location = LatLng(latitute, longitute)

            currentLocation = location
            mMap.moveCamera(CameraUpdateFactory.newLatLng(currentLocation))




//            Log.i("test", "Latitute: $latitute ; Longitute: $longitute")

        }

        override fun onStatusChanged(provider: String?, status: Int, extras: Bundle?) {
        }

        override fun onProviderEnabled(provider: String?) {
        }

        override fun onProviderDisabled(provider: String?) {
        }

    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == PERMISSION_REQUEST_ACCESS_FINE_LOCATION) {
            when (grantResults[0]) {
                PackageManager.PERMISSION_GRANTED -> getLocation()
                //PackageManager.PERMISSION_DENIED -> //Tell to user the need of grant permission
            }
        }
    }

    companion object {
        private const val PERMISSION_REQUEST_ACCESS_FINE_LOCATION = 100
    }
}
